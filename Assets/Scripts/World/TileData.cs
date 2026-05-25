namespace PlanetCore
{
    public sealed class TileData
    {
        public int            WorldX     { get; }
        public int            WorldY     { get; }
        public TileDefinition Definition { get; }

        public IStructureComponent PlacedStructure { get; set; }
        public bool                IsEmpty         => PlacedStructure == null;

        public string TileTypeId         => Definition.TileTypeId;
        public bool   HasTag(string tag) => Definition.HasTag(tag);

        public TileData(int worldX, int worldY, TileDefinition definition)
        {
            WorldX     = worldX;
            WorldY     = worldY;
            Definition = definition;
        }
    }
}