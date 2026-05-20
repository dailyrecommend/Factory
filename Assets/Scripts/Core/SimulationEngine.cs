namespace PlanetCore
{
    public class SimulationEngine
    {
        private readonly GameSystems _sys;

        public SimulationEngine(GameSystems sys) => _sys = sys;

        public void Tick(float realDeltaTime)
        {
            if (_sys.TurnManager.CurrentState != SessionState.RealTimePhase) return;

            _sys.Synergy.ResolveSynergies();

            float totalProduced = 0f;
            foreach (var tile in _sys.World.AllOccupiedTiles())
            {
                var b = tile.PlacedBuilding;
                if (b == null || !b.IsOperational) continue;
                totalProduced += b.Tick(realDeltaTime, _sys.World);
            }

            if (totalProduced > 0f)
                _sys.Battery.Receive(totalProduced);

            _sys.TurnManager.Tick(realDeltaTime);
        }
    }
}
