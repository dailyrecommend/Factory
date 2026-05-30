using System;
using UnityEngine;

namespace PlanetCore
{
    // Stores energy produced by generators.
    // Automatically sells all stored energy every SellInterval seconds.
    public sealed class EnergyPool
    {
        public float TotalCapacity { get; private set; } = 0f;
        public float StoredEnergy  { get; private set; } = 0f;

        public bool  IsFull    => TotalCapacity > 0f && StoredEnergy >= TotalCapacity;
        public bool  HasSpace  => StoredEnergy < TotalCapacity;
        public float FillRatio => TotalCapacity <= 0f ? 0f : StoredEnergy / TotalCapacity;

        public event Action<float> OnStoredChanged;
        public event Action<float> OnCapacityChanged;
        public event Action<float> OnEnergySold;      // fired when energy converts to credits

        private readonly EconomyState _economy;
        private readonly IGameConfig  _config;
        private GlobalStats _globalStats;
        private float                 _sellTimer;

        public EnergyPool(EconomyState economy, IGameConfig config, GlobalStats globalStats)
        {
            _economy      = economy;
            _config       = config;
            _globalStats  = globalStats;
            _sellTimer    = config.SellInterval;
        }

        // Called by SimulationEngine every tick
        public void Tick(float deltaTime)
        {
            if (StoredEnergy <= 0f) return;

            _sellTimer -= deltaTime;
            if (_sellTimer > 0f) return;

            _sellTimer = _config.SellInterval;
            SellAll();
        }

        private void SellAll()
        {
            if (StoredEnergy <= 0f) return;

            // Apply global EPC multiplier
            float credits = StoredEnergy * _config.EnergyToCredits * _globalStats.EPCMultiplier;
            float sold    = StoredEnergy;

            StoredEnergy = 0f;
            OnStoredChanged?.Invoke(StoredEnergy);

            _economy.AddCredits(credits);
            OnEnergySold?.Invoke(sold);
        }

        public void AddCapacity(float amount)
        {
            if (amount <= 0f) return;
            TotalCapacity += amount;
            OnCapacityChanged?.Invoke(TotalCapacity);
        }

        public void RemoveCapacity(float amount)
        {
            if (amount <= 0f) return;
            TotalCapacity = Math.Max(0f, TotalCapacity - amount);

            if (StoredEnergy > TotalCapacity)
            {
                StoredEnergy = TotalCapacity;
                OnStoredChanged?.Invoke(StoredEnergy);
            }

            OnCapacityChanged?.Invoke(TotalCapacity);
        }

        public void Add(float amount)
        {
            if (amount <= 0f)        return;
            if (TotalCapacity <= 0f) return;

            float room    = TotalCapacity - StoredEnergy;
            float toStore = Math.Min(amount, room);
            if (toStore <= 0f) return;

            StoredEnergy += toStore;
            OnStoredChanged?.Invoke(StoredEnergy);
        }

        public void HardReset()
        {
            TotalCapacity = 0f;
            StoredEnergy  = 0f;
            _sellTimer    = _config.SellInterval;
            OnStoredChanged?.Invoke(StoredEnergy);
            OnCapacityChanged?.Invoke(TotalCapacity);
        }
        
        public float SellTimerRatio
        {
            get
            {
                if (_config.SellInterval <= 0f) return 0f;
                float elapsed = _config.SellInterval - _sellTimer;
                return Mathf.Clamp01(elapsed / _config.SellInterval);
            }
        }
    }
}