using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PlanetCore
{
    public sealed class HUDView : MonoBehaviour
    {
        [Header("Battery")]
        [SerializeField] private Image            _batteryFill;
        [SerializeField] private TextMeshProUGUI  _batteryText;

        [Header("Sell Timer")]
        [SerializeField] private Image            _sellTimerFill;

        [Header("Credits")]
        [SerializeField] private TextMeshProUGUI  _creditsText;

        [Header("EPS")]
        [SerializeField] private TextMeshProUGUI  _epsText;

        private EconomyState     _economy;
        private EnergyPool       _energyPool;
        private SimulationEngine _simulationEngine;

        public void Init(
            EconomyState economy,
            EnergyPool energyPool,
            SimulationEngine simulationEngine)
        {
            _economy          = economy;
            _energyPool       = energyPool;
            _simulationEngine = simulationEngine;

            _economy.OnCreditsChanged     += OnCreditsChanged;
            _energyPool.OnStoredChanged   += OnEnergyChanged;
            _energyPool.OnCapacityChanged += OnEnergyChanged;

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
        }

        private void Update()
        {
            RefreshSellTimer();
            RefreshEPS();
        }

        private void OnCreditsChanged(float _) => RefreshCredits();
        private void OnEnergyChanged(float _)  => RefreshBattery();

        private void RefreshAll()
        {
            RefreshBattery();
            RefreshCredits();
            RefreshEPS();
        }

        private void RefreshBattery()
        {
            if (_batteryFill != null)
                _batteryFill.fillAmount = _energyPool.FillRatio;

            if (_batteryText != null)
                _batteryText.text = $"{_energyPool.StoredEnergy:F0} / {_energyPool.TotalCapacity:F0}";
        }

        private void RefreshSellTimer()
        {
            if (_sellTimerFill != null)
                _sellTimerFill.fillAmount = _energyPool.SellTimerRatio;
        }

        private void RefreshCredits()
        {
            if (_creditsText != null)
                _creditsText.text = $"{_economy.Credits:N0}";
        }

        private void RefreshEPS()
        {
            if (_epsText != null)
                _epsText.text = $"{_simulationEngine.CurrentEPS:F1}";
        }
    }
}