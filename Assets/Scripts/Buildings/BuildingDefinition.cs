using System;

namespace PlanetCore
{
    public class BuildingDefinition
    {
        public string   BuildingId      { get; }
        public string   DisplayName     { get; }
        public TerrainType? RequiredTerrain { get; }
        public Func<IBuildingBehavior> Factory { get; }

        public BuildingDefinition(
            string buildingId,
            string displayName,
            Func<IBuildingBehavior> factory,
            TerrainType? requiredTerrain = null)
        {
            BuildingId      = buildingId;
            DisplayName     = displayName;
            Factory         = factory;
            RequiredTerrain = requiredTerrain;
        }
    }
}
