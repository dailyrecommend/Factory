using System;

namespace PlanetCore
{
    public static class QuotaCalculator
    {
        /// <summary>Q(T) = BaseQuota × 1.20^(T-1)</summary>
        public static float Compute(float baseQuota, int turn)
            => baseQuota * (float)Math.Pow(GameConstants.QuotaGrowthRate, turn - 1);
    }
}
