using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetCore
{
    // Controls simulation speed: Paused / Normal(1x) / Fast(2x).
    // Uses Time.timeScale so animations pause automatically.
    // UI and camera use Time.unscaledDeltaTime so they remain responsive.
    public sealed class GameSpeedController : MonoBehaviour
    {
        public enum SpeedMode { Paused, Normal, Fast }

        public SpeedMode CurrentMode { get; private set; } = SpeedMode.Normal;

        public event Action<SpeedMode> OnSpeedChanged;

        private float _baseFixedDeltaTime;

        private void Awake()
        {
            _baseFixedDeltaTime = Time.fixedDeltaTime;
            Apply(SpeedMode.Normal);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.digit1Key.wasPressedThisFrame) Apply(SpeedMode.Paused);
            if (keyboard.digit2Key.wasPressedThisFrame) Apply(SpeedMode.Normal);
            if (keyboard.digit3Key.wasPressedThisFrame) Apply(SpeedMode.Fast);
        }

        private void Apply(SpeedMode mode)
        {
            CurrentMode = mode;

            float scale = mode switch
            {
                SpeedMode.Paused => 0f,
                SpeedMode.Normal => 1f,
                SpeedMode.Fast   => 2f,
                _                => 1f
            };

            Time.timeScale      = scale;
            Time.fixedDeltaTime = scale > 0f
                ? _baseFixedDeltaTime * scale
                : _baseFixedDeltaTime;

            OnSpeedChanged?.Invoke(CurrentMode);
        }

        private void OnDestroy()
        {
            Time.timeScale      = 1f;
            Time.fixedDeltaTime = _baseFixedDeltaTime;
        }
    }
}