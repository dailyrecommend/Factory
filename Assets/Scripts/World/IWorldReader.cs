namespace PlanetCore
{
    // Read-only world access interface.
    // Modules and resolvers depend on this, not on WorldMap directly.
    public interface IWorldReader
    {
        bool            IsTileUnlocked (int worldX, int worldY);
        TileData        GetTile        (int worldX, int worldY);
        IStructureComponent GetBuildingAt  (int worldX, int worldY);
    }
}