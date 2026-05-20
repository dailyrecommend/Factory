using System;

namespace PlanetCore
{
    public class GameSession
    {
        public GameSystems Systems { get; private set; }

        private SimulationEngine            _engine;
        private readonly CardPool           _cardPool;
        private readonly MetaProgressionManager _meta;
        private Random                      _rng;

        public event Action<SettlementResult> OnSettlement;
        public event Action                   OnGameOverConfirmed;

        public GameSession(int seed, MetaProgressionManager meta = null)
        {
            _meta     = meta ?? new MetaProgressionManager();
            _cardPool = new CardPool();
            _rng      = new Random(seed);

            Build(seed);
        }

        public void Tick(float realDeltaTime) => _engine.Tick(realDeltaTime);

        public SettlementResult DeliverAtMidnight()
        {
            var result = Systems.TurnManager.ExecuteSettlement();
            OnSettlement?.Invoke(result);
            return result;
        }

        public void TriggerManualBurst()
        {
            int bx = GameConstants.BasecampLocalX;
            int by = GameConstants.BasecampLocalY;

            if (Systems.World.TryGetTile(bx, by, out var tile)
                && tile.PlacedBuilding is IManualBurstBehavior burst)
            {
                burst.TriggerBurst();
            }
        }

        public bool TryExpandChunk(int chunkX, int chunkY)
        {
            if (!Systems.PendingChunkExpansion) return false;

            foreach (var (cx, cy) in Systems.World.GetExpandableCandidates())
            {
                if (cx != chunkX || cy != chunkY) continue;

                Systems.World.UnlockChunk(chunkX, chunkY);
                Systems.PendingChunkExpansion = false;
                return true;
            }
            return false;
        }

        public bool TryPickRewardCard(string cardId)
            => Systems.Rewards.TryPickCard(cardId, Systems.TurnManager);

        public bool TryRerollReward()
            => Systems.Rewards.TryReroll(_rng);

        private void Build(int seed)
        {
            var economy   = new EconomyState();
            var battery   = new BatteryState(500f);
            var generator = new ChunkGenerator(seed);
            var world     = new WorldMap(seed, generator);
            var catalog   = new BuildingCatalog();
            var placement = new PlacementService(world, catalog, economy);
            var synergy   = new SynergyResolver(world);
            var turnMgr   = new TurnManager(economy, battery, 100f);
            var rewards   = new RewardSystem(_cardPool, economy);

            Systems = new GameSystems(economy, battery, world, generator,
                                      turnMgr, placement, synergy, rewards);

            rewards.SetSystems(Systems);
            _engine = new SimulationEngine(Systems);

            turnMgr.OnRewardPhaseStarted  += () => rewards.GenerateOffer(_rng);
            turnMgr.OnSettlementCompleted += r  => OnSettlement?.Invoke(r);
            turnMgr.OnGameOver            += HandleGameOver;

            _meta.ReapplyToNewSession(_cardPool, rewards, Systems);
        }

        private void HandleGameOver()
        {
            _meta.SnapshotFromRewardSystem(Systems.Rewards);

            Systems.Economy.Reset();
            Systems.Battery.DrainAll();
            Systems.World.HardReset();
            Systems.TurnManager.SessionReset();
            Systems.Rewards.SessionReset();
            Systems.Placement.ResetCount();

            _rng = new Random(Environment.TickCount);

            _meta.ReapplyToNewSession(_cardPool, Systems.Rewards, Systems);

            OnGameOverConfirmed?.Invoke();
        }
    }
}
