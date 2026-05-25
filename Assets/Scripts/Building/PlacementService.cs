using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetCore
{
    public sealed class PlacementService
    {
        private readonly WorldMap         _world;
        private readonly DataRegistry     _registry;
        private readonly EconomyState     _economy;
        private readonly ComponentContext _componentCtx;
        private readonly ChunkExpander _chunkExpander;
        private readonly WorldRenderer _worldRenderer;


        private readonly Dictionary<string, int> _placedCounts = new();

        public int GetPlacedCount(string structureId)
            => _placedCounts.TryGetValue(structureId, out var count) ? count : 0;

        public PlacementService(
            WorldMap world, DataRegistry registry,
            EconomyState economy, ComponentContext componentCtx,
            ChunkExpander chunkExpander, WorldRenderer worldRenderer)
        {
            _world         = world;
            _registry      = registry;
            _economy       = economy;
            _componentCtx  = componentCtx;
            _chunkExpander = chunkExpander;
            _worldRenderer = worldRenderer;
        }

        public float NextStructureCost(string structureId)
        {
            if (!_registry.TryGetStructure(structureId, out var def)) return 0f;
            float multiplier = (float)Math.Pow(
                _registry.Config.BuildingCostExponent,
                GetPlacedCount(structureId));
            return def.Cost * multiplier;
        }

        public PlacementResult TryPlace(string structureId, int worldX, int worldY)
        {
            if (!_registry.TryGetStructure(structureId, out var def))
                return PlacementResult.TagRequirementNotMet;

            if (!_world.TryGetTile(worldX, worldY, out var tile))
                return PlacementResult.ChunkNotUnlocked;

            if (!tile.IsEmpty)
                return PlacementResult.TileOccupied;

            if (def.HasTileRequirement && !tile.HasTag(def.RequiredTileTag))
                return PlacementResult.TagRequirementNotMet;

            if (_economy.Credits < NextStructureCost(structureId))
                return PlacementResult.InsufficientCredits;

            var position  = TileToWorld(worldX, worldY);
            var go        = InstantiatePrefab(def.PrefabPath, position);
            var structure = go.GetComponent<IStructureComponent>();

            if (structure == null)
            {
                UnityEngine.Object.Destroy(go);
                throw new Exception($"[PlacementService] No IStructureComponent on: {def.PrefabPath}");
            }

            if (structure is BrokerComponent broker)
                broker.Setup(_chunkExpander, _worldRenderer);
            
            structure.WorldPosition = (worldX, worldY);
            tile.PlacedStructure    = structure;
            structure.OnPlaced(tile, _componentCtx);
            
            var animator = go.GetComponent<StructureAnimator>();
            animator?.PlayPlaceAnimation();
            
            _economy.SpendCredits(NextStructureCost(structureId));
            
            if (!_placedCounts.ContainsKey(structureId))
                _placedCounts[structureId] = 0;
            _placedCounts[structureId]++;

            return PlacementResult.Success;
        }

        public bool TryDemolish(int worldX, int worldY)
        {
            if (!_world.TryGetTile(worldX, worldY, out var tile)) return false;
            if (tile.IsEmpty)                                       return false;
            if (tile.PlacedStructure.StructureId == "basecamp")    return false;

            tile.PlacedStructure.OnRemoved(tile, _componentCtx);

            if (tile.PlacedStructure is MonoBehaviour mb)
                UnityEngine.Object.Destroy(mb.gameObject);

            float refund = NextStructureCost(tile.PlacedStructure.StructureId)
                           * _registry.Config.DemolishRefundRatio;
            _economy.AddCredits(refund);
            
            tile.PlacedStructure = null;
            
            var id = tile.PlacedStructure.StructureId;
            if (_placedCounts.ContainsKey(id))
                _placedCounts[id] = Math.Max(0, _placedCounts[id] - 1);
            
            return true;
        }

        private Vector3 TileToWorld(int worldX, int worldY)
            => new Vector3(
                worldX * _registry.Config.TileSize,
                0f,
                worldY * _registry.Config.TileSize);

        private static GameObject InstantiatePrefab(string prefabPath, Vector3 position)
        {
            var prefab = Resources.Load<GameObject>(prefabPath);

            if (prefab != null)
                return UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);

            Debug.LogError($"[PlacementService] Prefab not found: {prefabPath}");
            throw new Exception($"[PlacementService] Missing prefab: {prefabPath}");
        }

        public void ResetCount() => _placedCounts.Clear();
    }
}