using System;

namespace PlanetCore
{
    public class BatteryState
    {
        public float MaxCapacity         { get; private set; }
        public float Stored              { get; private set; }
        public float OverproductionStack { get; private set; }
        public float TotalDeliverable    => Stored + OverproductionStack;
        public float FillRatio           => MaxCapacity > 0f ? Stored / MaxCapacity : 0f;

        public event Action<float, float> OnEnergyChanged;

        public BatteryState(float maxCapacity)
        {
            MaxCapacity = maxCapacity;
        }

        public void Receive(float energy)
        {
            float headroom = MaxCapacity - Stored;

            if (energy <= headroom)
            {
                Stored += energy;
            }
            else
            {
                OverproductionStack += energy - headroom;
                Stored               = MaxCapacity;
            }

            OnEnergyChanged?.Invoke(Stored, OverproductionStack);
        }

        public void ExpandCapacity(float delta) => MaxCapacity += delta;

        public void DrainAll()
        {
            Stored              = 0f;
            OverproductionStack = 0f;
            OnEnergyChanged?.Invoke(Stored, OverproductionStack);
        }
    }
}
