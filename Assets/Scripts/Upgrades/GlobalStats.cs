using System;

namespace PlanetCore
{
    // Holds globally modified stats from upgrades.
    // Uses additive bonus: EPSBonus = 0.02 + 0.02 = 0.04 → multiplier = 1.04
    public sealed class GlobalStats
    {
        private float _epsBonus = 0f;
        private float _epcBonus = 0f;

        public float EPSMultiplier => 1f + _epsBonus;
        public float EPCMultiplier => 1f + _epcBonus;

        public event Action OnStatsChanged;

        public void AddEPSBonus(float bonus)
        {
            _epsBonus += bonus;
            OnStatsChanged?.Invoke();
        }

        public void AddEPCBonus(float bonus)
        {
            _epcBonus += bonus;
            OnStatsChanged?.Invoke();
        }
    }
}