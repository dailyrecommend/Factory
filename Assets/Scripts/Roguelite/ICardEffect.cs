namespace PlanetCore
{
    public interface ICardEffect
    {
        string CardId { get; }
        void Apply    (GameSystems systems);
        void OnUpgrade(int newLevel, GameSystems systems);
    }
}
