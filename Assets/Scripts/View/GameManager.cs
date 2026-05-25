using UnityEngine;

namespace PlanetCore
{
    public sealed class GameManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private WorldRenderer    _worldRenderer;
        [SerializeField] private PlacementManager _placementManager;
        [SerializeField] private HUDView          _hudView;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private SimulationEngine _simulationEngine;

        private IGameConfig      _config;
        private DataRegistry     _registry;
        private WorldMap         _world;
        private EconomyState     _economy;
        private EnergyPool       _energyPool;
        private PlacementService _placementService;
        private SynergyResolver  _synergyResolver;
        private TurnManager      _turnManager;
        private ChunkExpander _chunkExpander;

        private void Awake()
        {
            _config   = new GameConfig();
            _registry = new DataRegistry(_config);

            _economy    = new EconomyState(_config.StartingCredits);
            _energyPool = new EnergyPool();

            
            
            var generator = new ChunkGenerator(Random.Range(0, int.MaxValue), _registry);
            _world        = new WorldMap(42, generator, _config, _registry);
            _chunkExpander    = new ChunkExpander(_world);
            
            var componentCtx  = new ComponentContext(_world, _config, _energyPool);
            _placementService = new PlacementService(_world, _registry, _economy, componentCtx, _chunkExpander, _worldRenderer);
            _synergyResolver  = new SynergyResolver(_world);

            var quotaCalc = new QuotaCalculator(_config);
            _turnManager  = new TurnManager(_config, quotaCalc, _energyPool, _economy);

            _worldRenderer.Init(_world, _config);
            _placementManager.Init(_placementService, _registry);
            _hudView.Init(_economy, _energyPool, _turnManager);
            _cameraController.Init(_world, _config);
            _simulationEngine.Init(
                _world, _energyPool, _synergyResolver, componentCtx, _turnManager);

            Debug.Log("[GameManager] Ready.");
        }
    }
}