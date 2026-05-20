using System.Collections.Generic;

namespace PlanetCore
{
    public class BuildingCatalog
    {
        private readonly Dictionary<string, BuildingDefinition> _defs
            = new Dictionary<string, BuildingDefinition>();

        public BuildingCatalog()
        {
            Register(new BuildingDefinition(
                "coal_generator", "Coal Generator",
                () => new CoalGenerator()));

            Register(new BuildingDefinition(
                "coal_extractor", "Coal Extractor",
                () => new CoalExtractor(),
                TerrainType.ResourceDeposit));
        }

        public void Register(BuildingDefinition def) => _defs[def.BuildingId] = def;

        public bool TryGet(string id, out BuildingDefinition def)
            => _defs.TryGetValue(id, out def);

        public IEnumerable<BuildingDefinition> AllDefinitions => _defs.Values;
    }
}
