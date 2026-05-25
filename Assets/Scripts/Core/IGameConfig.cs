namespace PlanetCore
{
    // Read-only contract for all game-wide numeric constants.
    // Injected via constructor. Never accessed through a static singleton.
    public interface IGameConfig
    {
        // World
        int   ChunkSize      { get; }
        
        float StartingCredits { get; }
        
        float DepositChance  { get; }

        // Basecamp
        float ManualBurstDuration     { get; }
        float ManualBurstEnergyPerSec { get; }

        // Economy
        float BaseQuota          { get; }
        float QuotaGrowthRate    { get; }
        float BaseRate           { get; }
        float BonusRate          { get; }

        // Building
        float DemolishRefundRatio  { get; }
        float BuildingCostBase     { get; }
        float BuildingCostExponent { get; }

        // Time
        float DayLengthSeconds { get; }

        // Rendering
        float TileSize { get; }
    }
}