using System;

namespace PlanetCore
{
    public class PlacementService
    {
        private readonly WorldMap        _world;
        private readonly BuildingCatalog _catalog;
        private readonly EconomyState    _economy;

        public int TotalPlacedCount { get; private set; } = 0;

        public PlacementService(WorldMap world, BuildingCatalog catalog, EconomyState economy)
        {
            _world   = world;
            _catalog = catalog;
            _economy = economy;
        }

        public float NextBuildCost()
            => (float)(GameConstants.BuildingCostBase
               * Math.Pow(GameConstants.BuildingCostExponent, TotalPlacedCount));

        public float DemolishRefund()
            => NextBuildCost() * GameConstants.DemolishRefundRatio;

        public PlacementResult TryPlace(string buildingId, int worldX, int worldY)
        {
            if (!_catalog.TryGet(buildingId, out var def))
                return PlacementResult.InvalidTerrain;

            if (!_world.TryGetTile(worldX, worldY, out var tile))
                return PlacementResult.ChunkNotUnlocked;

            if (tile.PlacedBuilding != null)
                return PlacementResult.TileOccupied;

            if (def.RequiredTerrain.HasValue && tile.TerrainType != def.RequiredTerrain.Value)
                return PlacementResult.InvalidTerrain;

            float cost = NextBuildCost();
            if (_economy.Credits < cost)
                return PlacementResult.InsufficientCredits;

            var instance = def.Factory();
            instance.WorldPosition = (worldX, worldY);

            if (instance is CoalExtractor extractor)
                extractor.ValidatePlacement(_world);

            tile.PlacedBuilding = instance;
            _economy.SpendCredits(cost);
            TotalPlacedCount++;

            return PlacementResult.Success;
        }

        public bool TryDemolish(int worldX, int worldY)
        {
            if (!_world.TryGetTile(worldX, worldY, out var tile)) return false;
            if (tile.PlacedBuilding == null)                       return false;
            if (tile.PlacedBuilding is Basecamp)                   return false;

            float refund = DemolishRefund();
            tile.PlacedBuilding = null;
            TotalPlacedCount    = Math.Max(0, TotalPlacedCount - 1);
            _economy.AddCredits(refund);
            return true;
        }

        public void ResetCount() => TotalPlacedCount = 0;
    }
}
