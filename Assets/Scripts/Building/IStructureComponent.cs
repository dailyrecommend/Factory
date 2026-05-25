using UnityEngine;

namespace PlanetCore
{
    // Contract for all structure components attached to prefabs as MonoBehaviour.
    // Each component is responsible for one specific behaviour.
    // PlacementService calls OnPlaced/OnRemoved. SimulationEngine calls Tick every frame.
    public interface IStructureComponent
    {
        string StructureId   { get; }
        string DisplayName   { get; }
        bool   IsOperational { get; }

        (int WorldX, int WorldY) WorldPosition { get; set; }

        void  OnPlaced (TileData tile, ComponentContext ctx);
        void  OnRemoved(TileData tile, ComponentContext ctx);
        float Tick     (float deltaTime, ComponentContext ctx);

        void  ReceiveSynergyBonus(float bonusRatio);
        void  ClearSynergyBonuses();
        float SynergyBonusRatio { get; }
        bool  EmitsSynergy      { get; }
    }
}