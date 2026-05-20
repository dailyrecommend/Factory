namespace PlanetCore
{
    public class SynergyResolver
    {
        private readonly WorldMap _world;

        private static readonly (int dx, int dy)[] Neighbours8 =
        {
            (-1,-1),(0,-1),(1,-1),
            (-1, 0),      (1, 0),
            (-1, 1),(0, 1),(1, 1)
        };

        public SynergyResolver(WorldMap world) => _world = world;

        public void ResolveSynergies()
        {
            foreach (var tile in _world.AllOccupiedTiles())
                tile.PlacedBuilding?.ClearSynergyBonuses();

            foreach (var tile in _world.AllOccupiedTiles())
            {
                if (tile.PlacedBuilding is not IExtractorBehavior extractor) continue;
                if (!extractor.IsOperational || !extractor.IsMining)          continue;

                var (ex, ey) = extractor.WorldPosition;

                foreach (var (dx, dy) in Neighbours8)
                {
                    if (!_world.TryGetTile(ex + dx, ey + dy, out var n)) continue;
                    if (n.PlacedBuilding == null)                          continue;
                    if (n.PlacedBuilding is IExtractorBehavior)            continue;

                    n.PlacedBuilding.ReceiveSynergyBonus(extractor.SynergyBonusRatio);
                }
            }
        }
    }
}
