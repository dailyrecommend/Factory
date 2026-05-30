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
        [SerializeField] private UpgradePanel _upgradePanel;
        
        private IGameConfig      _config;
        private DataRegistry     _registry;
        private WorldMap         _world;
        private EconomyState     _economy;
        private EnergyPool       _energyPool;
        private PlacementService _placementService;
        private SynergyResolver  _synergyResolver;
        private ChunkExpander    _chunkExpander;
        private GlobalStats    _globalStats;
        private UpgradeSystem  _upgradeSystem;

        private void Awake()
        {
            _config   = new GameConfig();
            _registry = new DataRegistry(_config);

            _economy     = new EconomyState(_config.StartingCredit);
            _globalStats = new GlobalStats();
            _energyPool  = new EnergyPool(_economy, _config, _globalStats);

            var generator = new ChunkGenerator(Random.Range(0, int.MaxValue), _registry);
            _world        = new WorldMap(42, generator, _config, _registry);

            var componentCtx  = new ComponentContext(_world, _config, _energyPool);
            _chunkExpander    = new ChunkExpander(_world);
            _placementService = new PlacementService(
                _world, _registry, _economy, componentCtx,
                _chunkExpander, _worldRenderer);
            _synergyResolver  = new SynergyResolver(_world);

            _upgradeSystem = new UpgradeSystem(_economy, _globalStats, _world, _placementService);
            _upgradeSystem.LoadFromResources();

            _worldRenderer.Init(_world, _config);
            _placementManager.Init(_placementService, _registry);
            _hudView.Init(_economy, _energyPool, _simulationEngine);
            _cameraController.Init(_world, _config);
            _simulationEngine.Init(_world, _energyPool, _synergyResolver, componentCtx, _globalStats);
            _upgradePanel.Init(_upgradeSystem, _simulationEngine, _energyPool);
            
            Debug.Log("[GameManager] Ready.");
        }
    }
}