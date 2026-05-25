using UnityEngine;

namespace PlanetCore
{
    // When placed, unlocks a random adjacent chunk.
    // When removed, deactivates the linked chunk.
    // Links 1:1 with the chunk it unlocked.
    public sealed class BrokerComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId = "broker";
        [SerializeField] private string _displayName = "Broker";

        public string StructureId   => _structureId;
        public string DisplayName   => _displayName;
        public bool   IsOperational { get; private set; } = false;
        public bool   EmitsSynergy  => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }

        // The chunk this broker has unlocked
        public Chunk LinkedChunk { get; private set; }

        private ChunkExpander  _expander;
        private WorldRenderer  _worldRenderer;

        public void Setup(ChunkExpander expander, WorldRenderer worldRenderer)
        {
            _expander      = expander;
            _worldRenderer = worldRenderer;
        }

        public void OnPlaced(TileData tile, ComponentContext ctx)
        {
            if (_expander == null)
            {
                Debug.LogError("[BrokerComponent] ChunkExpander not set. Call Setup() first.");
                return;
            }

            LinkedChunk = _expander.UnlockRandom();

            if (LinkedChunk != null)
            {
                _worldRenderer.SpawnChunkView(LinkedChunk);
                IsOperational = true;
            }
        }

        public void OnRemoved(TileData tile, ComponentContext ctx)
        {
            if (LinkedChunk != null)
            {
                _expander.Deactivate(LinkedChunk);
                _worldRenderer.RefreshChunkView(LinkedChunk);
            }

            IsOperational = false;
        }

        public float Tick(float deltaTime, ComponentContext ctx) => 0f;

        public void ReceiveSynergyBonus(float bonusRatio) { }
        public void ClearSynergyBonuses()                 { }
    }
}