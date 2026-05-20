namespace PlanetCore
{
    public class CoalExtractor : BuildingBase, IExtractorBehavior
    {
        public override string BuildingId  => "coal_extractor";
        public override string DisplayName => "Coal Extractor";

        public ResourceType ResourceType      => ResourceType.Coal;
        public float        SynergyBonusRatio => 0.50f;
        public bool         IsMining          { get; private set; }

        public void ValidatePlacement(IWorldContext context)
        {
            IsMining = context.GetTerrainType (WorldPosition.WorldX, WorldPosition.WorldY) == TerrainType.ResourceDeposit
                    && context.GetResourceType(WorldPosition.WorldX, WorldPosition.WorldY) == ResourceType.Coal;

            IsOperational = IsMining;
        }

        public override float Tick(float deltaTime, IWorldContext context) => 0f;
    }
}
