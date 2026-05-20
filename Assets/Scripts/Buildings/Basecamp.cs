using System;

namespace PlanetCore
{
    public class Basecamp : BuildingBase, IManualBurstBehavior
    {
        public override string BuildingId  => "basecamp";
        public override string DisplayName => "Manual Generator";

        public float BurstTimeRemaining { get; private set; } = 0f;

        public override void ReceiveSynergyBonus(float bonusRatio) { }
        public override void ClearSynergyBonuses() { }

        public void TriggerBurst()
            => BurstTimeRemaining = GameConstants.ManualBurstDuration;

        public override float Tick(float deltaTime, IWorldContext context)
        {
            if (BurstTimeRemaining <= 0f) return 0f;

            float elapsed      = Math.Min(deltaTime, BurstTimeRemaining);
            BurstTimeRemaining = Math.Max(0f, BurstTimeRemaining - deltaTime);

            return EmitEnergy(GameConstants.ManualBurstEnergyPerSec * elapsed);
        }
    }
}
