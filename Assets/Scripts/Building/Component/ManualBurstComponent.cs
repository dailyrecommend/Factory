using System;
using UnityEngine;

namespace PlanetCore
{
    public sealed class ManualBurstComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId = "basecamp";
        [SerializeField] private string _displayName = "Basecamp";

        public string StructureId   => _structureId;
        public string DisplayName   => _displayName;
        public bool   IsOperational => true;
        public bool   EmitsSynergy  => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }
        public float BurstTimeRemaining { get; private set; } = 0f;

        public void TriggerBurst(float duration) => BurstTimeRemaining = duration;

        public void OnPlaced (TileData tile, ComponentContext ctx) { }
        public void OnRemoved(TileData tile, ComponentContext ctx)
            => BurstTimeRemaining = 0f;

        public float Tick(float deltaTime, ComponentContext ctx)
        {
            if (BurstTimeRemaining <= 0f) return 0f;
            float elapsed      = Math.Min(deltaTime, BurstTimeRemaining);
            BurstTimeRemaining = Math.Max(0f, BurstTimeRemaining - deltaTime);
            return ctx.Config.ManualBurstEnergyPerSec * elapsed;
        }

        public void ReceiveSynergyBonus(float bonusRatio) { }
        public void ClearSynergyBonuses()                 { }
    }
}