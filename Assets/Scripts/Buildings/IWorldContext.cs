namespace PlanetCore
{
    public interface IWorldContext
    {
        bool              IsTileUnlocked  (int worldX, int worldY);
        IBuildingBehavior GetBuildingAt   (int worldX, int worldY);
        TerrainType       GetTerrainType  (int worldX, int worldY);
        ResourceType      GetResourceType (int worldX, int worldY);
    }
}
