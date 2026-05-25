using UnityEngine;

namespace PlanetCore
{
    public sealed class TileTagRequirementComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId  = "";
        [SerializeField] private string _displayName  = "";
        [SerializeField] private string _requiredTag  = "";

        public string StructureId   => _structureId;
        public string DisplayName   => _displayName;
        public bool   IsOperational { get; private set; } = false;
        public bool   EmitsSynergy  => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }

        public void OnPlaced(TileData tile, ComponentContext ctx)
            => IsOperational = string.IsNullOrEmpty(_requiredTag) || tile.HasTag(_requiredTag);

        public void OnRemoved(TileData tile, ComponentContext ctx)
            => IsOperational = false;

        public float Tick(float deltaTime, ComponentContext ctx) => 0f;

        public void ReceiveSynergyBonus(float bonusRatio) { }
        public void ClearSynergyBonuses()                 { }
    }
}