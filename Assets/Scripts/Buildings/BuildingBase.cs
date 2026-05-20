using System;

namespace PlanetCore
{
    public abstract class BuildingBase : IBuildingBehavior
    {
        public abstract string BuildingId  { get; }
        public abstract string DisplayName { get; }

        public (int WorldX, int WorldY) WorldPosition { get; set; }
        public bool IsOperational { get; protected set; } = true;

        protected float _synergyRatio = 0f;

        public event Action<float> OnEnergyProduced;

        public virtual void ReceiveSynergyBonus(float bonusRatio) => _synergyRatio += bonusRatio;
        public virtual void ClearSynergyBonuses()                 => _synergyRatio  = 0f;

        public abstract float Tick(float deltaTime, IWorldContext context);

        protected float EmitEnergy(float amount)
        {
            if (amount > 0f) OnEnergyProduced?.Invoke(amount);
            return amount;
        }
    }
}
