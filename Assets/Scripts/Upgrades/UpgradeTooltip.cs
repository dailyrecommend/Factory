using TMPro;
using UnityEngine;

namespace PlanetCore
{
    // Follows mouse position and displays upgrade info on hover.
    public sealed class UpgradeTooltip : MonoBehaviour
    {
        [SerializeField] private GameObject      _root;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private RectTransform   _rectTransform;

        private void Awake() => Hide();

        public void Show(UpgradeDefinition def, Vector2 screenPos)
        {
            _root.SetActive(true);
            _nameText.text = def.DisplayName;
            _descText.text = $"{def.Description}\n{def.EffectDescription}";
            _costText.text = $"{def.Cost:N0} Credits";

            // Position tooltip near cursor with screen boundary clamp
            var canvas     = GetComponentInParent<Canvas>();
            var canvasRect = canvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, canvas.worldCamera, out var localPos);

            localPos += new Vector2(20f, -20f); // offset from cursor
            _rectTransform.anchoredPosition = localPos;
        }

        public void Hide() => _root.SetActive(false);
    }
}