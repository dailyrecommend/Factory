using System;

namespace PlanetCore
{
    // Calculates the daily energy quota and credits earned.
    // Pure function-style class; dependencies injected via constructor.
    public sealed class QuotaCalculator
    {
        private readonly IGameConfig _config;

        public QuotaCalculator(IGameConfig config) => _config = config;

        public float CalculateQuota(int dayNumber)
        {
            // Geometric growth: BaseQuota * (1 + GrowthRate)^(day-1)
            if (dayNumber <= 1) return _config.BaseQuota;
            return (float)(_config.BaseQuota *
                           Math.Pow(1.0 + _config.QuotaGrowthRate, dayNumber - 1));
        }

        // Credits earned from a settlement. Bonus rate applies to surplus above quota.
        public float CalculateCredits(float energyDelivered, float quota)
        {
            if (energyDelivered <= 0f || quota <= 0f) return 0f;

            float baseAmount  = Math.Min(energyDelivered, quota) * _config.BaseRate;
            float surplus     = Math.Max(0f, energyDelivered - quota);
            float bonusRatio  = surplus / quota;
            float bonusAmount = surplus * bonusRatio;

            return baseAmount + bonusAmount;
        }
    }
}