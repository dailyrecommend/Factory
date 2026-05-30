namespace PlanetCore
{
    public sealed class TileData
    {
        public int            WorldX     { get; }
        public int            WorldY     { get; }
        public TileDefinition Definition { get; }
        public Chunk          Chunk      { get; }

        public IStructureComponent PlacedStructure { get; set; }
        public bool                IsEmpty         => PlacedStructure == null;

        // Persists across broker demolish/replace cycles
        public Chunk LinkedChunk { get; set; }

        public string TileTypeId         => Definition.TileTypeId;
        public bool   HasTag(string tag) => Definition.HasTag(tag);

        public TileData(int worldX, int worldY, TileDefinition definition, Chunk chunk)
        {
            WorldX     = worldX;
            WorldY     = worldY;
            Definition = definition;
            Chunk      = chunk;
        }
    }
}