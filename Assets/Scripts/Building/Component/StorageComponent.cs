using UnityEngine;

namespace PlanetCore
{
    public sealed class StorageComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId = "energy_storage";
        [SerializeField] private string _displayName = "Energy Storage";
        [SerializeField] private float  _capacity    = 100f;

        public string StructureId   => _structureId;
        public string DisplayName   => _displayName;
        public bool   IsOperational { get; private set; } = false;
        public bool   EmitsSynergy  => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }

        private EnergyPool _energyPool;

        public void OnPlaced(TileData tile, ComponentContext ctx)
        {
            _energyPool = ctx.EnergyPool;
            _energyPool.AddCapacity(_capacity);
            IsOperational = true;
        }

        public void OnRemoved(TileData tile, ComponentContext ctx)
        {
            _energyPool?.RemoveCapacity(_capacity);
            IsOperational = false;
        }

        public void OnChunkActivated()
        {
            _energyPool?.AddCapacity(_capacity);
            IsOperational = true;
        }

        public void OnChunkDeactivated()
        {
            _energyPool?.RemoveCapacity(_capacity);
            IsOperational = false;
        }

        public void AddCapacity(float amount)
        {
            _capacity += amount;
            if (IsOperational)
                _energyPool?.AddCapacity(amount);
        }
        
        public float Tick(float deltaTime, ComponentContext ctx) => 0f;

        public void ReceiveSynergyBonus(float bonusRatio) { }
        public void ClearSynergyBonuses()                 { }
    }
}