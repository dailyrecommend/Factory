namespace PlanetCore
{
    public interface IGameConfig
    {
        int   ChunkSize               { get; }
        float DepositChance           { get; }
        int StartingCredit            { get;  }
        float ManualBurstDuration     { get; }
        float ManualBurstEnergyPerSec { get; }
        float DemolishRefundRatio     { get; }
        float BuildingCostExponent    { get; }
        float TileSize                { get; }
        float SellInterval            { get; }
        float EnergyToCredits         { get; }
    }
}