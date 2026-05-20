namespace PlanetCore
{
    public class ChunkExpansionEffect : ICardEffect
    {
        public string CardId => "chunk_expansion";

        public void Apply(GameSystems systems)
            => systems.PendingChunkExpansion = true;

        public void OnUpgrade(int newLevel, GameSystems systems) { }
    }
}
