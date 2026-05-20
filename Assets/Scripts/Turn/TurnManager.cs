using System;

namespace PlanetCore
{
    public class TurnManager
    {
        public SessionState CurrentState      { get; private set; } = SessionState.RealTimePhase;
        public int          TurnNumber        { get; private set; } = 1;
        public bool         IsReprieve        { get; private set; } = false;
        public float        ElapsedDayTime    { get; private set; } = 0f;
        public float        SpeedMultiplier   { get; private set; } = 1f;
        public bool         TimeAccelUnlocked { get; private set; } = false;

        public float CurrentQuota => QuotaCalculator.Compute(_baseQuota, TurnNumber);

        private readonly EconomyState _economy;
        private readonly BatteryState _battery;
        private readonly float        _baseQuota;

        public event Action<SessionState>     OnStateChanged;
        public event Action                   OnRewardPhaseStarted;
        public event Action<SettlementResult> OnSettlementCompleted;
        public event Action                   OnGameOver;

        public TurnManager(EconomyState economy, BatteryState battery, float baseQuota = 100f)
        {
            _economy   = economy;
            _battery   = battery;
            _baseQuota = baseQuota;
        }

        public void Tick(float realDeltaTime)
        {
            if (CurrentState != SessionState.RealTimePhase) return;

            ElapsedDayTime += realDeltaTime * SpeedMultiplier;

            if (ElapsedDayTime >= GameConstants.DayLengthSeconds)
            {
                ElapsedDayTime = GameConstants.DayLengthSeconds;
                Transition(SessionState.SettlementPhase);
            }
        }

        public bool TrySetSpeed(float multiplier)
        {
            if (!TimeAccelUnlocked || multiplier <= 0f) return false;
            SpeedMultiplier = multiplier;
            return true;
        }

        public void UnlockTimeAccelerator() => TimeAccelUnlocked = true;

        public SettlementResult ExecuteSettlement()
        {
            if (CurrentState != SessionState.SettlementPhase)
                throw new InvalidOperationException("Settlement called outside SettlementPhase.");

            float quota     = CurrentQuota;
            float delivered = _battery.TotalDeliverable;

            SettlementResult result = delivered >= quota
                ? ProcessSuccess(quota, delivered)
                : ProcessFailure(quota, delivered);

            OnSettlementCompleted?.Invoke(result);
            return result;
        }

        private SettlementResult ProcessSuccess(float quota, float delivered)
        {
            float surplus = delivered - quota;
            float earned  = quota * GameConstants.BaseRate + surplus * GameConstants.BonusRate;

            _economy.AddCredits(earned);
            _battery.DrainAll();

            TurnNumber++;
            IsReprieve     = false;
            ElapsedDayTime = 0f;

            Transition(SessionState.RewardPhase);
            OnRewardPhaseStarted?.Invoke();

            return new SettlementResult(true, quota, delivered, earned, 0f, TurnNumber, false);
        }

        private SettlementResult ProcessFailure(float quota, float delivered)
        {
            float earned  = delivered * GameConstants.BaseRate;
            float penalty = quota - delivered;

            _economy.AddCredits(earned);
            _economy.SpendCredits(penalty);
            _battery.DrainAll();
            ElapsedDayTime = 0f;

            if (IsReprieve)
            {
                var goResult = new SettlementResult(false, quota, delivered, earned, penalty, TurnNumber, true);
                Transition(SessionState.GameOver);
                OnGameOver?.Invoke();
                return goResult;
            }

            IsReprieve = true;
            Transition(SessionState.ReprievePhase);
            return new SettlementResult(false, quota, delivered, earned, penalty, TurnNumber, false);
        }

        public void ResumeFromReward()
        {
            if (CurrentState != SessionState.RewardPhase) return;
            Transition(SessionState.RealTimePhase);
        }

        public void ResumeFromReprieve()
        {
            if (CurrentState != SessionState.ReprievePhase) return;
            Transition(SessionState.RealTimePhase);
        }

        public void SessionReset()
        {
            TurnNumber      = 1;
            IsReprieve      = false;
            ElapsedDayTime  = 0f;
            SpeedMultiplier = 1f;
            CurrentState    = SessionState.RealTimePhase;
        }

        private void Transition(SessionState next)
        {
            CurrentState = next;
            OnStateChanged?.Invoke(next);
        }
    }
}
