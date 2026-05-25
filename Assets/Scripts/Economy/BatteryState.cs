using System;

namespace PlanetCore
{
    // Tracks accumulated energy and an "overproduction stack"
    // that holds excess energy when the battery is full.
    // Stored energy is consumed at settlement to fulfill the quota.
    public sealed class BatteryState
    {
        public float StoredEnergy        { get; private set; }
        public float MaxCapacity         { get; private set; }
        public float OverproductionStack { get; private set; }

        public bool  IsFull              => StoredEnergy >= MaxCapacity;
        public float FillRatio           => MaxCapacity <= 0f ? 0f : StoredEnergy / MaxCapacity;

        public event Action<float> OnStoredChanged;
        public event Action<float> OnOverproductionChanged;

        public BatteryState(float maxCapacity)
        {
            MaxCapacity = maxCapacity;
        }

        // Adds energy. Excess goes into the overproduction stack.
        public void Add(float amount)
        {
            if (amount <= 0f) return;

            float room        = MaxCapacity - StoredEnergy;
            float toStore     = Math.Min(amount, room);
            float toOverflow  = amount - toStore;

            if (toStore > 0f)
            {
                StoredEnergy += toStore;
                OnStoredChanged?.Invoke(StoredEnergy);
            }

            if (toOverflow > 0f)
            {
                OverproductionStack += toOverflow;
                OnOverproductionChanged?.Invoke(OverproductionStack);
            }
        }

        // Drains up to `amount` from stored energy first, then from overproduction.
        // Returns the actual amount drained.
        public float Drain(float amount)
        {
            if (amount <= 0f) return 0f;

            float drained = 0f;

            float fromStored = Math.Min(amount, StoredEnergy);
            if (fromStored > 0f)
            {
                StoredEnergy -= fromStored;
                drained      += fromStored;
                amount       -= fromStored;
                OnStoredChanged?.Invoke(StoredEnergy);
            }

            if (amount > 0f && OverproductionStack > 0f)
            {
                float fromStack = Math.Min(amount, OverproductionStack);
                OverproductionStack -= fromStack;
                drained             += fromStack;
                OnOverproductionChanged?.Invoke(OverproductionStack);
            }

            return drained;
        }

        public float TotalAvailable() => StoredEnergy + OverproductionStack;

        public void SetMaxCapacity(float newMax)
        {
            MaxCapacity = Math.Max(0f, newMax);
            if (StoredEnergy > MaxCapacity)
            {
                float overflow = StoredEnergy - MaxCapacity;
                StoredEnergy   = MaxCapacity;
                OverproductionStack += overflow;
                OnStoredChanged?.Invoke(StoredEnergy);
                OnOverproductionChanged?.Invoke(OverproductionStack);
            }
        }

        public void HardReset(float maxCapacity)
        {
            MaxCapacity         = maxCapacity;
            StoredEnergy        = 0f;
            OverproductionStack = 0f;
            OnStoredChanged?.Invoke(StoredEnergy);
            OnOverproductionChanged?.Invoke(OverproductionStack);
        }
    }
}