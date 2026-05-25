using System;

namespace PlanetCore
{
    // Tracks player credits.
    // Pure C# domain class — no MonoBehaviour, no static singletons.
    public sealed class EconomyState
    {
        public float Credits { get; private set; }

        public event Action<float> OnCreditsChanged;

        public EconomyState(float startingCredits = 0f)
            => Credits = startingCredits;

        public void AddCredits(float amount)
        {
            if (amount <= 0f) return;
            Credits += amount;
            OnCreditsChanged?.Invoke(Credits);
        }

        public bool SpendCredits(float amount)
        {
            if (amount <= 0f)        return true;
            if (amount > Credits)    return false;

            Credits -= amount;
            OnCreditsChanged?.Invoke(Credits);
            return true;
        }

        public bool CanAfford(float amount) => Credits >= amount;

        public void HardReset(float startingCredits = 0f)
        {
            Credits = startingCredits;
            OnCreditsChanged?.Invoke(Credits);
        }
    }
}