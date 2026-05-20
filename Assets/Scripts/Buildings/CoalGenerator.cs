namespace PlanetCore
{
    public class CoalGenerator : BuildingBase
    {
        public override string BuildingId  => "coal_generator";
        public override string DisplayName => "Coal Generator";

        public float BaseEnergyPerSecond { get; }

        public CoalGenerator(float baseEnergyPerSecond = 10f)
            => BaseEnergyPerSecond = baseEnergyPerSecond;

        public override float Tick(float deltaTime, IWorldContext context)
        {
            float multiplier = 1f + _synergyRatio;
            return EmitEnergy(BaseEnergyPerSecond * multiplier * deltaTime);
        }
    }
}
