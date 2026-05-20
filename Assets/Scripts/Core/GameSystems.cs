namespace PlanetCore
{
    public class GameSystems
    {
        public EconomyState     Economy        { get; }
        public BatteryState     Battery        { get; }
        public WorldMap         World          { get; }
        public ChunkGenerator   ChunkGenerator { get; }
        public TurnManager      TurnManager    { get; }
        public PlacementService Placement      { get; }
        public SynergyResolver  Synergy        { get; }
        public RewardSystem     Rewards        { get; }

        public bool PendingChunkExpansion { get; set; } = false;

        public GameSystems(
            EconomyState economy, BatteryState battery,
            WorldMap world, ChunkGenerator generator,
            TurnManager turnManager, PlacementService placement,
            SynergyResolver synergy, RewardSystem rewards)
        {
            Economy        = economy;
            Battery        = battery;
            World          = world;
            ChunkGenerator = generator;
            TurnManager    = turnManager;
            Placement      = placement;
            Synergy        = synergy;
            Rewards        = rewards;
        }
    }
}
