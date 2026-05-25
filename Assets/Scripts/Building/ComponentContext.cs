namespace PlanetCore
{
    // Passed to IStructureComponent on every Tick, OnPlaced, and OnRemoved call.
    public sealed class ComponentContext
    {
        public IWorldReader World      { get; }
        public IGameConfig  Config     { get; }
        public EnergyPool   EnergyPool { get; }

        public ComponentContext(IWorldReader world, IGameConfig config, EnergyPool energyPool)
        {
            World      = world;
            Config     = config;
            EnergyPool = energyPool;
        }
    }
}