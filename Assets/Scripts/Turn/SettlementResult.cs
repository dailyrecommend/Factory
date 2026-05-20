namespace PlanetCore
{
    public class SettlementResult
    {
        public bool  WasSuccess      { get; }
        public float Quota           { get; }
        public float TotalDelivered  { get; }
        public float Surplus         { get; }
        public float CreditsEarned   { get; }
        public float CreditsDeducted { get; }
        public int   TurnNumber      { get; }
        public bool  IsGameOver      { get; }

        public SettlementResult(bool success, float quota, float delivered,
            float earned, float deducted, int turn, bool gameOver)
        {
            WasSuccess      = success;
            Quota           = quota;
            TotalDelivered  = delivered;
            Surplus         = delivered - quota;
            CreditsEarned   = earned;
            CreditsDeducted = deducted;
            TurnNumber      = turn;
            IsGameOver      = gameOver;
        }
    }
}
