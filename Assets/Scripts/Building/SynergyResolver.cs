namespace PlanetCore
{
    public sealed class SynergyResolver
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
                tile.PlacedStructure?.ClearSynergyBonuses();

            foreach (var tile in _world.AllOccupiedTiles())
            {
                var structure = tile.PlacedStructure;
                if (structure == null || !structure.IsOperational) continue;
                if (!structure.EmitsSynergy)                       continue;

                var (ex, ey) = structure.WorldPosition;

                foreach (var (dx, dy) in Neighbours8)
                {
                    if (!_world.TryGetTile(ex + dx, ey + dy, out var neighbour)) continue;
                    if (neighbour.IsEmpty)                                         continue;
                    if (neighbour.PlacedStructure.EmitsSynergy)                   continue;

                    neighbour.PlacedStructure.ReceiveSynergyBonus(structure.SynergyBonusRatio);
                }
            }
        }
    }
}