using UnityEngine;

namespace PlanetCore
{
    public class GameManager : MonoBehaviour
    {
        [Header("World Seed")]
        [SerializeField] private int _seed = 114514;

        public static GameManager Instance { get; private set; }

        public GameSession      Session   { get; private set; }
        public GameSystems      Systems   => Session.Systems;
        public EconomyState     Economy   => Session.Systems.Economy;
        public BatteryState     Battery   => Session.Systems.Battery;
        public TurnManager      Turn      => Session.Systems.TurnManager;
        public PlacementService Placement => Session.Systems.Placement;
        public WorldMap         World     => Session.Systems.World;
        public RewardSystem     Rewards   => Session.Systems.Rewards;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Session = new GameSession(_seed);
            Session.OnSettlement        += OnSettlement;
            Session.OnGameOverConfirmed += OnGameOver;

            Debug.Log($"[GameManager] Planet '{World.PlanetName}' initialised. Seed: {_seed}");
        }

        private void Update() => Session.Tick(Time.deltaTime);

        private void OnDestroy()
        {
            if (Session == null) return;
            Session.OnSettlement        -= OnSettlement;
            Session.OnGameOverConfirmed -= OnGameOver;
        }

        public void Deliver()                        => Session.DeliverAtMidnight();
        public void TriggerBurst()                   => Session.TriggerManualBurst();
        public void PickCard(string cardId)          => Session.TryPickRewardCard(cardId);
        public void Reroll()                         => Session.TryRerollReward();
        public void ExpandChunk(int cx, int cy)      => Session.TryExpandChunk(cx, cy);

        private void OnSettlement(SettlementResult r)
        {
            if (r.WasSuccess)
                Debug.Log($"[T{r.TurnNumber - 1}] SUCCESS  +{r.CreditsEarned:F0}cr  surplus {r.Surplus:F0}");
            else if (r.IsGameOver)
                Debug.Log("[Settlement] GAME OVER");
            else
                Debug.Log($"[T{r.TurnNumber}] FAIL  -{r.CreditsDeducted:F0}cr  reprieve active");
        }

        private void OnGameOver() =>
            Debug.Log("[GameManager] Game over. Meta saved. Session reset.");
    }
}
