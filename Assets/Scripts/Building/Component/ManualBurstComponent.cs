using System;
using UnityEngine;

namespace PlanetCore
{
    public sealed class ManualBurstComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId = "basecamp";
        [SerializeField] private string _displayName = "Basecamp";

        public string StructureId   => _structureId;
        public string DisplayName   => _displayName;
        public bool   IsOperational { get; private set; } = true;
        public bool   EmitsSynergy  => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }
        public float BurstTimeRemaining { get; private set; } = 0f;

        public void TriggerBurst(float duration) => BurstTimeRemaining = duration;

        public void OnPlaced (TileData tile, ComponentContext ctx) { }
        public void OnRemoved(TileData tile, ComponentContext ctx)
            => BurstTimeRemaining = 0f;

        public void OnChunkActivated()   => IsOperational = true;
        public void OnChunkDeactivated()
        {
            BurstTimeRemaining = 0f;
            IsOperational      = false;
        }

        public float Tick(float deltaTime, ComponentContext ctx)
        {
            if (!IsOperational || BurstTimeRemaining <= 0f) return 0f;
            float elapsed      = Math.Min(deltaTime, BurstTimeRemaining);
            BurstTimeRemaining = Math.Max(0f, BurstTimeRemaining - deltaTime);
            return ctx.Config.ManualBurstEnergyPerSec * elapsed;
        }

        public void ReceiveSynergyBonus(float bonusRatio) { }
        public void ClearSynergyBonuses()                 { }
    }
}