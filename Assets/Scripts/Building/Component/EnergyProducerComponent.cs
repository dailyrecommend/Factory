using UnityEngine;

namespace PlanetCore
{
    public sealed class EnergyProducerComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId = "generator";
        [SerializeField] private string _displayName = "Generator";
        [SerializeField] private float  _baseOutput  = 10f;

        private float _synergyRatio = 0f;

        public string StructureId   => _structureId;
        public string DisplayName   => _displayName;
        public bool   IsOperational { get; private set; } = true;
        public bool   EmitsSynergy  => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }

        public void OnPlaced (TileData tile, ComponentContext ctx) { }
        public void OnRemoved(TileData tile, ComponentContext ctx) { }

        public void OnChunkActivated()   => IsOperational = true;
        public void OnChunkDeactivated() => IsOperational = false;

        public float Tick(float deltaTime, ComponentContext ctx)
        {
            if (!IsOperational) return 0f;
            return _baseOutput * (1f + _synergyRatio) * deltaTime;
        }

        public void ReceiveSynergyBonus(float bonusRatio) => _synergyRatio += bonusRatio;
        public void ClearSynergyBonuses()                 => _synergyRatio  = 0f;
    }
}