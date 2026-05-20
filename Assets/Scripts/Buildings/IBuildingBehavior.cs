using System;

namespace PlanetCore
{
    public interface IBuildingBehavior
    {
        string BuildingId  { get; }
        string DisplayName { get; }

        (int WorldX, int WorldY) WorldPosition { get; set; }

        bool IsOperational { get; }

        float Tick(float deltaTime, IWorldContext context);

        void ReceiveSynergyBonus(float bonusRatio);
        void ClearSynergyBonuses();

        event Action<float> OnEnergyProduced;
    }
}
