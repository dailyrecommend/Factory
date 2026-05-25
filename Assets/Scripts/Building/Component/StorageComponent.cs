using UnityEngine;

namespace PlanetCore
{
    // Adds energy storage capacity to the EnergyPool when placed.
    // Attach to any structure prefab that should store energy.
    public sealed class StorageComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId  = "energy_storage";
        [SerializeField] private string _displayName  = "Energy Storage";
        [SerializeField] private float  _capacity     = 100f;

        public string StructureId   => _structureId;
        public string DisplayName   => _displayName;
        public bool   IsOperational { get; private set; } = false;
        public bool   EmitsSynergy  => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }

        public void OnPlaced(TileData tile, ComponentContext ctx)
        {
            ctx.EnergyPool.AddCapacity(_capacity);
            IsOperational = true;
        }

        public void OnRemoved(TileData tile, ComponentContext ctx)
        {
            ctx.EnergyPool.RemoveCapacity(_capacity);
            IsOperational = false;
        }

        public float Tick(float deltaTime, ComponentContext ctx) => 0f;

        public void ReceiveSynergyBonus(float bonusRatio) { }
        public void ClearSynergyBonuses()                 { }
    }
}