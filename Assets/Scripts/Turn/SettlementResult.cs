namespace PlanetCore
{
    // Immutable value object describing the outcome of a daily settlement.
    // Produced by TurnManager.ExecuteSettlement and consumed by UI/RewardSystem.
    public sealed class SettlementResult
    {
        public int   DayNumber       { get; }
        public float Quota           { get; }
        public float EnergyDelivered { get; }
        public float EnergyShortfall { get; }
        public bool  QuotaMet        { get; }
        public float CreditsEarned   { get; }
        public float OverproductionUsed { get; }

        public SettlementResult(
            int   dayNumber,
            float quota,
            float energyDelivered,
            float energyShortfall,
            bool  quotaMet,
            float creditsEarned,
            float overproductionUsed)
        {
            DayNumber          = dayNumber;
            Quota              = quota;
            EnergyDelivered    = energyDelivered;
            EnergyShortfall    = energyShortfall;
            QuotaMet           = quotaMet;
            CreditsEarned      = creditsEarned;
            OverproductionUsed = overproductionUsed;
        }
    }
}