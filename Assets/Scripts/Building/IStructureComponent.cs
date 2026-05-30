namespace PlanetCore
{
    public interface IStructureComponent
    {
        string StructureId    { get; }
        string DisplayName    { get; }
        bool   IsOperational  { get; }

        (int WorldX, int WorldY) WorldPosition { get; set; }

        void  OnPlaced          (TileData tile, ComponentContext ctx);
        void  OnRemoved         (TileData tile, ComponentContext ctx);
        float Tick              (float deltaTime, ComponentContext ctx);

        void  OnChunkActivated  ();
        void  OnChunkDeactivated();

        void  ReceiveSynergyBonus(float bonusRatio);
        void  ClearSynergyBonuses();
        float SynergyBonusRatio  { get; }
        bool  EmitsSynergy       { get; }
    }
}