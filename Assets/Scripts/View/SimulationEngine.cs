using UnityEngine;

namespace PlanetCore
{
    public sealed class SimulationEngine : MonoBehaviour
    {
        private WorldMap         _world;
        private EnergyPool       _energyPool;
        private SynergyResolver  _synergyResolver;
        private ComponentContext _componentCtx;
        private GlobalStats _globalStats;

        private float _energyAccumulator = 0f;
        private float _timeAccumulator   = 0f;
        private float _currentEPS        = 0f;
        private const float EPS_UPDATE_INTERVAL = 1f;

        public float CurrentEPS => _currentEPS;
        
        public void Init(
            WorldMap world,
            EnergyPool energyPool,
            SynergyResolver synergyResolver,
            ComponentContext componentCtx,
            GlobalStats globalStats)
        {
            _world           = world;
            _energyPool      = energyPool;
            _synergyResolver = synergyResolver;
            _componentCtx    = componentCtx;
            _globalStats     = globalStats;
        }

        private void Update()
        {
            _synergyResolver.ResolveSynergies();

            float totalEnergy = 0f;
            foreach (var tile in _world.AllOccupiedTiles())
            {
                var structure = tile.PlacedStructure;
                if (structure == null || !structure.IsOperational) continue;

                // Apply global EPS multiplier
                totalEnergy += structure.Tick(Time.deltaTime, _componentCtx)
                               * _globalStats.EPSMultiplier;
            }

            if (totalEnergy > 0f)
                _energyPool.Add(totalEnergy);

            _energyPool.Tick(Time.deltaTime);

            // EPS tracking
            _energyAccumulator += totalEnergy;
            _timeAccumulator   += Time.deltaTime;
            if (_timeAccumulator >= EPS_UPDATE_INTERVAL)
            {
                _currentEPS        = _energyAccumulator / _timeAccumulator;
                _energyAccumulator = 0f;
                _timeAccumulator   = 0f;
            }
        }
    }
}