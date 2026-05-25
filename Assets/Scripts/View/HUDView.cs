using UnityEngine;
using UnityEngine.UI;

namespace PlanetCore
{
    public sealed class HUDView : MonoBehaviour
    {
        [SerializeField] private Text _creditsText;
        [SerializeField] private Text _energyText;
        [SerializeField] private Text _quotaText;
        [SerializeField] private Text _dayText;

        private EconomyState _economy;
        private EnergyPool   _energyPool;
        private TurnManager  _turnManager;

        public void Init(EconomyState economy, EnergyPool energyPool, TurnManager turnManager)
        {
            _economy     = economy;
            _energyPool  = energyPool;
            _turnManager = turnManager;

            _economy.OnCreditsChanged     += OnCreditsChanged;
            _energyPool.OnStoredChanged   += OnEnergyChanged;
            _energyPool.OnCapacityChanged += OnEnergyChanged;

            _turnManager.OnDayStarted         += OnDayStarted;
            _turnManager.OnSettlementCompleted += OnSettlementCompleted;

            RefreshAll();
        }

        private void OnDestroy()
        {
            if (_economy != null)
                _economy.OnCreditsChanged -= OnCreditsChanged;

            if (_energyPool != null)
            {
                _energyPool.OnStoredChanged   -= OnEnergyChanged;
                _energyPool.OnCapacityChanged -= OnEnergyChanged;
            }

            if (_turnManager != null)
            {
                _turnManager.OnDayStarted          -= OnDayStarted;
                _turnManager.OnSettlementCompleted -= OnSettlementCompleted;
            }
        }

        // Time-based update only for day progress
        private void Update()
        {
            if (_turnManager == null) return;
            RefreshDay();
        }

        private void OnCreditsChanged(float _)    => RefreshCredits();
        private void OnEnergyChanged(float _)      => RefreshEnergy();
        private void OnDayStarted(int _)           => RefreshDay();
        private void OnSettlementCompleted(SettlementResult _) => RefreshAll();

        private void RefreshAll()
        {
            RefreshCredits();
            RefreshEnergy();
            RefreshQuota();
            RefreshDay();
        }

        private void RefreshCredits()
        {
            if (_creditsText != null)
                _creditsText.text = $"Credits: {_economy.Credits:F0}";
        }

        private void RefreshEnergy()
        {
            if (_energyText != null)
                _energyText.text = $"Energy: {_energyPool.StoredEnergy:F0} / {_energyPool.TotalCapacity:F0}";
        }

        private void RefreshQuota()
        {
            if (_quotaText != null)
                _quotaText.text = $"Quota: {_turnManager.CurrentQuota:F0}";
        }

        private void RefreshDay()
        {
            if (_dayText == null) return;
            float remaining = _turnManager.Config.DayLengthSeconds
                            * (1f - _turnManager.DayProgress);
            _dayText.text = $"Day {_turnManager.DayNumber} — {remaining:F0}s";
        }
    }
}