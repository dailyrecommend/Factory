using UnityEngine;

namespace PlanetCore
{
    public sealed class SynergyEmitterComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId       = "extractor";
        [SerializeField] private string _displayName       = "Extractor";
        [SerializeField] private float  _synergyBonusRatio = 0.5f;
        [SerializeField] private string _requiredTag       = "mineable";

        public string StructureId      => _structureId;
        public string DisplayName      => _displayName;
        public bool   IsOperational    { get; private set; } = false;
        public bool   EmitsSynergy     => IsOperational;
        public float  SynergyBonusRatio => _synergyBonusRatio;

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