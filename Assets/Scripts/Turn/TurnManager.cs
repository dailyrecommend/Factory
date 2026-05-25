using System;

namespace PlanetCore
{
    public sealed class TurnManager
    {
        private readonly IGameConfig     _config;
        private readonly QuotaCalculator _quota;
        private readonly EnergyPool      _energyPool;
        private readonly EconomyState    _economy;

        public IGameConfig Config      => _config;
        public int         DayNumber   { get; private set; } = 1;
        public float       ElapsedDayTime { get; private set; } = 0f;
        public float       SpeedMultiplier { get; set; } = 1f;
        public bool        IsGameOver  { get; private set; } = false;

        public float CurrentQuota => _quota.CalculateQuota(DayNumber);
        public float DayProgress  => _config.DayLengthSeconds <= 0f
            ? 0f : Math.Min(1f, ElapsedDayTime / _config.DayLengthSeconds);

        public event Action<int>              OnDayStarted;
        public event Action<SettlementResult> OnSettlementCompleted;
        public event Action                   OnGameOver;

        public TurnManager(
            IGameConfig config,
            QuotaCalculator quota,
            EnergyPool energyPool,
            EconomyState economy)
        {
            _config     = config;
            _quota      = quota;
            _energyPool = energyPool;
            _economy    = economy;
        }

        public bool Tick(float realDeltaTime)
        {
            if (IsGameOver) return false;

            ElapsedDayTime += realDeltaTime * SpeedMultiplier;

            if (ElapsedDayTime < _config.DayLengthSeconds) return false;

            ElapsedDayTime = _config.DayLengthSeconds;
            return true;
        }

        public SettlementResult ExecuteSettlement()
        {
            float quota     = CurrentQuota;
            float available = _energyPool.StoredEnergy;
            float delivered = Math.Min(quota, available);
            float drained   = _energyPool.Drain(delivered);
            float shortfall = Math.Max(0f, quota - drained);
            bool  met       = shortfall <= 0f;
            float credits   = met ? _quota.CalculateCredits(drained, quota) : 0f;

            if (met) _economy.AddCredits(credits);

            var result = new SettlementResult(
                DayNumber, quota, drained, shortfall, met, credits, 0f);

            OnSettlementCompleted?.Invoke(result);

            if (!met)
            {
                IsGameOver = true;
                OnGameOver?.Invoke();
            }

            return result;
        }

        public void AdvanceToNextDay()
        {
            if (IsGameOver) return;
            DayNumber++;
            ElapsedDayTime = 0f;
            OnDayStarted?.Invoke(DayNumber);
        }

        public void HardReset()
        {
            DayNumber       = 1;
            ElapsedDayTime  = 0f;
            SpeedMultiplier = 1f;
            IsGameOver      = false;
            OnDayStarted?.Invoke(DayNumber);
        }
    }
}