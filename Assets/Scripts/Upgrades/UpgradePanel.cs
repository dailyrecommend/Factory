using UnityEngine;
using UnityEngine.UI;

namespace PlanetCore
{
    // Toggle panel showing available upgrades as icons.
    public sealed class UpgradePanel : MonoBehaviour
    {
        [SerializeField] private GameObject      _panelRoot;
        [SerializeField] private Transform       _iconContainer;
        [SerializeField] private GameObject      _iconPrefab;
        [SerializeField] private UpgradeTooltip  _tooltip;
        [SerializeField] private Button          _toggleButton;

        private UpgradeSystem    _upgradeSystem;
        private SimulationEngine _simulationEngine;
        private EnergyPool       _energyPool;

        public void Init(
            UpgradeSystem upgradeSystem,
            SimulationEngine simulationEngine,
            EnergyPool energyPool)
        {
            _upgradeSystem    = upgradeSystem;
            _simulationEngine = simulationEngine;
            _energyPool       = energyPool;

            _upgradeSystem.OnAvailableChanged += RefreshIcons;
            _toggleButton.onClick.AddListener(Toggle);

            _panelRoot.SetActive(false);
            RefreshIcons();
        }

        private void OnDestroy()
        {
            if (_upgradeSystem != null)
                _upgradeSystem.OnAvailableChanged -= RefreshIcons;
        }

        public void Toggle()
            => _panelRoot.SetActive(!_panelRoot.activeSelf);

        public void TryPurchase(string upgradeId)
        {
            _upgradeSystem.TryPurchase(
                upgradeId,
                _simulationEngine.CurrentEPS,
                _energyPool.TotalCapacity);
        }

        private void RefreshIcons()
        {
            // Clear existing icons
            foreach (Transform child in _iconContainer)
                Destroy(child.gameObject);

            var available = _upgradeSystem.GetAvailable(
                _simulationEngine.CurrentEPS,
                _energyPool.TotalCapacity);

            foreach (var def in available)
            {
                var go   = Instantiate(_iconPrefab, _iconContainer);
                var icon = go.GetComponent<UpgradeIcon>();
                icon.Init(def, _tooltip, this);
            }
        }
    }
}