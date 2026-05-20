namespace PlanetCore
{
    public interface IExtractorBehavior : IBuildingBehavior
    {
        ResourceType ResourceType      { get; }
        float        SynergyBonusRatio { get; }
        bool         IsMining          { get; }
    }
}
