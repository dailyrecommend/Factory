using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PlanetCore
{
    public sealed class UpgradeSystem
    {
        private readonly List<UpgradeDefinition> _all       = new();
        private readonly HashSet<string>         _purchased = new();
        private readonly EconomyState            _economy;
        private readonly GlobalStats             _globalStats;
        private readonly WorldMap                _world;
        private readonly PlacementService        _placement;

        public event Action<UpgradeDefinition> OnUpgradePurchased;
        public event Action                    OnAvailableChanged;

        public IReadOnlyList<UpgradeDefinition> All => _all;

        public UpgradeSystem(
            EconomyState economy,
            GlobalStats globalStats,
            WorldMap world,
            PlacementService placement)
        {
            _economy     = economy;
            _globalStats = globalStats;
            _world       = world;
            _placement   = placement;
        }

        public void LoadFromResources()
        {
            var rows = CsvParser.LoadFromResources("Data/upgrades");

            foreach (var row in rows)
            {
                var id          = row["upgradeId"];
                var name        = row["displayName"];
                var cost        = Parse(row["cost"]);
                var unlockType  = row["unlockType"] switch
                {
                    "eps"            => UnlockConditionType.EPS,
                    "capacity"       => UnlockConditionType.Capacity,
                    _                => UnlockConditionType.StructureCount
                };
                var unlockValue = Parse(row["unlockValue"]);
                var effectType  = row["effectType"] switch
                {
                    "eps_multiplier" => UpgradeEffectType.EPSMultiplier,
                    "epc_multiplier" => UpgradeEffectType.EPCMultiplier,
                    _                => UpgradeEffectType.StructureStat
                };
                var target      = row.TryGetValue("effectTarget", out var t) ? t : string.Empty;
                var value       = Parse(row["effectValue"]);
                var spriteName  = row.TryGetValue("spriteName",  out var s) ? s : string.Empty;
                var description = row.TryGetValue("description", out var d) ? d : string.Empty;

                _all.Add(new UpgradeDefinition(
                    id, name, cost, unlockType, unlockValue,
                    effectType, target, value,
                    spriteName, description));
            }
        }

        public bool IsPurchased(string upgradeId) => _purchased.Contains(upgradeId);

        public List<UpgradeDefinition> GetAvailable(float eps, float capacity)
        {
            var result = new List<UpgradeDefinition>();
            int totalPlaced = GetTotalPlacedCount();

            foreach (var def in _all)
            {
                if (_purchased.Contains(def.UpgradeId)) continue;
                if (IsUnlocked(def, eps, capacity, totalPlaced))
                    result.Add(def);
            }
            return result;
        }

        public bool TryPurchase(string upgradeId, float eps, float capacity)
        {
            var def = _all.Find(d => d.UpgradeId == upgradeId);
            if (def == null)                    return false;
            if (_purchased.Contains(upgradeId)) return false;

            int totalPlaced = GetTotalPlacedCount();
            if (!IsUnlocked(def, eps, capacity, totalPlaced)) return false;
            if (!_economy.SpendCredits(def.Cost))              return false;

            _purchased.Add(upgradeId);
            ApplyEffect(def);
            OnUpgradePurchased?.Invoke(def);
            OnAvailableChanged?.Invoke();

            Debug.Log($"[UpgradeSystem] Purchased: {def.DisplayName} ({def.EffectDescription})");
            return true;
        }

        private bool IsUnlocked(
            UpgradeDefinition def, float eps, float capacity, int structureCount)
            => def.UnlockType switch
            {
                UnlockConditionType.EPS            => eps            >= def.UnlockValue,
                UnlockConditionType.Capacity       => capacity       >= def.UnlockValue,
                UnlockConditionType.StructureCount => structureCount >= def.UnlockValue,
                _                                  => false
            };

        private void ApplyEffect(UpgradeDefinition def)
        {
            switch (def.EffectType)
            {
                case UpgradeEffectType.EPSMultiplier:
                    _globalStats.AddEPSBonus(def.EffectValue);
                    break;

                case UpgradeEffectType.EPCMultiplier:
                    _globalStats.AddEPCBonus(def.EffectValue);
                    break;

                case UpgradeEffectType.StructureStat:
                    ApplyStructureStat(def.EffectTarget, def.EffectValue);
                    break;
            }
        }

        private void ApplyStructureStat(string structureId, float value)
        {
            foreach (var tile in _world.AllOccupiedTiles())
            {
                if (tile.PlacedStructure?.StructureId != structureId) continue;
                if (tile.PlacedStructure is StorageComponent storage)
                    storage.AddCapacity(value);
            }
        }

        private int GetTotalPlacedCount()
        {
            int count = 0;
            foreach (var chunk in _world.AllUnlockedChunks())
                foreach (var tile in chunk.AllTiles())
                    if (!tile.IsEmpty) count++;
            return count;
        }

        private static float Parse(string s)
            => float.Parse(s, CultureInfo.InvariantCulture);
    }
}