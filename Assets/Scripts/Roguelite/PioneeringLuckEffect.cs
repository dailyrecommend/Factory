namespace PlanetCore
{
    public class PioneeringLuckEffect : ICardEffect
    {
        public string CardId => "pioneering_luck";

        public void Apply(GameSystems systems)
            => systems.ChunkGenerator.IncreaseMinDeposits(1);

        public void OnUpgrade(int newLevel, GameSystems systems)
            => systems.ChunkGenerator.IncreaseMinDeposits(1);
    }
}
