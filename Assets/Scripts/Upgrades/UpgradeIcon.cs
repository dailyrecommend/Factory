using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PlanetCore
{
    // Single upgrade icon in the upgrade panel.
    // Handles hover tooltip and click to purchase.
    public sealed class UpgradeIcon : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _borderImage;

        private UpgradeDefinition _def;
        private UpgradeTooltip    _tooltip;
        private UpgradePanel      _panel;

        // Border colors by effect type
        private static readonly Color ColorEPS     = new Color(0.4f, 0.8f, 0.4f);
        private static readonly Color ColorEPC     = new Color(0.4f, 0.6f, 1.0f);
        private static readonly Color ColorStorage = new Color(1.0f, 0.7f, 0.3f);

        public void Init(UpgradeDefinition def, UpgradeTooltip tooltip, UpgradePanel panel)
        {
            _def     = def;
            _tooltip = tooltip;
            _panel   = panel;

            // Load sprite
            var sprite = Resources.Load<Sprite>($"Sprites/Upgrades/{def.SpriteName}");
            if (sprite != null)
                _iconImage.sprite = sprite;

            // Set border color by effect type
            if (_borderImage != null)
                _borderImage.color = _def.EffectType switch
                {
                    UpgradeEffectType.EPSMultiplier => ColorEPS,
                    UpgradeEffectType.EPCMultiplier => ColorEPC,
                    UpgradeEffectType.StructureStat => ColorStorage,
                    _                               => Color.white
                };
        }

        public void OnPointerEnter(PointerEventData e)
            => _tooltip.Show(_def, e.position);

        public void OnPointerExit(PointerEventData e)
            => _tooltip.Hide();

        public void OnPointerClick(PointerEventData e)
        {
            _tooltip.Hide();
            _panel.TryPurchase(_def.UpgradeId);
        }
    }
}