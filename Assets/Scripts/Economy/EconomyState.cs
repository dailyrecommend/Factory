using System;

namespace PlanetCore
{
    public class EconomyState
    {
        public float Credits { get; private set; } = 0f;

        public event Action<float> OnCreditsChanged;

        public void AddCredits(float amount)
        {
            if (amount <= 0f) return;
            Credits += amount;
            OnCreditsChanged?.Invoke(Credits);
        }

        public void SpendCredits(float amount)
        {
            Credits = Math.Max(0f, Credits - amount);
            OnCreditsChanged?.Invoke(Credits);
        }

        public void Reset()
        {
            Credits = 0f;
            OnCreditsChanged?.Invoke(Credits);
        }
    }
}
