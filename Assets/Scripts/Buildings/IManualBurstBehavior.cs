namespace PlanetCore
{
    public interface IManualBurstBehavior : IBuildingBehavior
    {
        float BurstTimeRemaining { get; }
        void  TriggerBurst();
    }
}
