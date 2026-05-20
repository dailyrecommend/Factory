namespace PlanetCore
{
    public class TimeAcceleratorEffect : ICardEffect
    {
        public string CardId => "time_accelerator";

        public void Apply(GameSystems systems)
            => systems.TurnManager.UnlockTimeAccelerator();

        public void OnUpgrade(int newLevel, GameSystems systems) { }
    }
}
