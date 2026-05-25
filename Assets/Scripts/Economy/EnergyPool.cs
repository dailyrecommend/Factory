using System;

namespace PlanetCore
{
    // Tracks total energy capacity and stored energy across all placed EnergyStorage structures.
    // Capacity starts at 0. StorageComponent adds/removes capacity on placed/removed.
    public sealed class EnergyPool
    {
        public float TotalCapacity { get; private set; } = 0f;
        public float StoredEnergy  { get; private set; } = 0f;

        public bool  IsFull    => TotalCapacity > 0f && StoredEnergy >= TotalCapacity;
        public bool  HasSpace  => StoredEnergy < TotalCapacity;
        public float FillRatio => TotalCapacity <= 0f ? 0f : StoredEnergy / TotalCapacity;

        public event Action<float> OnStoredChanged;
        public event Action<float> OnCapacityChanged;

        // Called by StorageComponent.OnPlaced
        public void AddCapacity(float amount)
        {
            if (amount <= 0f) return;
            TotalCapacity += amount;
            OnCapacityChanged?.Invoke(TotalCapacity);
        }

        // Called by StorageComponent.OnRemoved
        public void RemoveCapacity(float amount)
        {
            if (amount <= 0f) return;
            TotalCapacity = Math.Max(0f, TotalCapacity - amount);

            // Clamp stored energy to new capacity
            if (StoredEnergy > TotalCapacity)
            {
                StoredEnergy = TotalCapacity;
                OnStoredChanged?.Invoke(StoredEnergy);
            }

            OnCapacityChanged?.Invoke(TotalCapacity);
        }

        // Called by SimulationEngine each tick with total energy produced
        public void Add(float amount)
        {
            if (amount <= 0f)        return;
            if (TotalCapacity <= 0f) return; // No storage available

            float room    = TotalCapacity - StoredEnergy;
            float toStore = Math.Min(amount, room);

            if (toStore <= 0f) return;

            StoredEnergy += toStore;
            OnStoredChanged?.Invoke(StoredEnergy);
        }

        // Called by TurnManager at settlement. Returns actual amount drained.
        public float Drain(float amount)
        {
            if (amount <= 0f) return 0f;

            float drained = Math.Min(amount, StoredEnergy);
            StoredEnergy -= drained;
            OnStoredChanged?.Invoke(StoredEnergy);
            return drained;
        }

        public void HardReset()
        {
            TotalCapacity = 0f;
            StoredEnergy  = 0f;
            OnStoredChanged?.Invoke(StoredEnergy);
            OnCapacityChanged?.Invoke(TotalCapacity);
        }
    }
}