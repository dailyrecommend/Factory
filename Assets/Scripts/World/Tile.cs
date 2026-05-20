namespace PlanetCore
{
    public class Tile
    {
        public int               WorldX         { get; }
        public int               WorldY         { get; }
        public TerrainType       TerrainType    { get; set; }
        public ResourceType      ResourceType   { get; set; }
        public IBuildingBehavior PlacedBuilding { get; set; }
        public bool              IsEmpty        => PlacedBuilding == null;

        public Tile(int worldX, int worldY,
                    TerrainType  terrain  = TerrainType.Plain,
                    ResourceType resource = ResourceType.None)
        {
            WorldX       = worldX;
            WorldY       = worldY;
            TerrainType  = terrain;
            ResourceType = resource;
        }
    }
}
