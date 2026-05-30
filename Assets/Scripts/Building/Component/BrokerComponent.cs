using UnityEngine;

namespace PlanetCore
{
    public sealed class BrokerComponent : MonoBehaviour, IStructureComponent
    {
        [SerializeField] private string _structureId = "broker";
        [SerializeField] private string _displayName = "Broker";

        public string StructureId    => _structureId;
        public string DisplayName    => _displayName;
        public bool   IsOperational  { get; private set; } = false;
        public bool   EmitsSynergy   => false;
        public float  SynergyBonusRatio => 0f;

        public (int WorldX, int WorldY) WorldPosition { get; set; }

        private ChunkExpander _expander;
        private WorldRenderer _worldRenderer;
        private TileData      _tile;

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

            _tile = tile;

            if (tile.LinkedChunk == null)
            {
                // First time — unlock a new chunk and store it on the tile
                tile.LinkedChunk = _expander.UnlockRandom();
            }
            else
            {
                // Reinstall — reactivate the already linked chunk
                _expander.Activate(tile.LinkedChunk);
            }

            if (tile.LinkedChunk != null)
            {
                bool isNewChunk = !_worldRenderer.HasChunkView(tile.LinkedChunk);
                if (isNewChunk)
                    _worldRenderer.SpawnChunkView(tile.LinkedChunk);
                else
                    _worldRenderer.RefreshChunkView(tile.LinkedChunk);

                IsOperational = true;
            }
        }

        public void OnRemoved(TileData tile, ComponentContext ctx)
        {
            if (tile.LinkedChunk != null)
            {
                _expander.Deactivate(tile.LinkedChunk);
                _worldRenderer.RefreshChunkView(tile.LinkedChunk);
            }

            IsOperational = false;
        }

        public void OnChunkDeactivated()
        {
            if (_tile?.LinkedChunk == null) return;
            _expander.Deactivate(_tile.LinkedChunk);
            _worldRenderer.RefreshChunkView(_tile.LinkedChunk);
            IsOperational = false;
        }

        public void OnChunkActivated()
        {
            if (_tile?.LinkedChunk == null) return;
            _expander.Activate(_tile.LinkedChunk);
            _worldRenderer.RefreshChunkView(_tile.LinkedChunk);
            IsOperational = true;
        }

        public float Tick(float deltaTime, ComponentContext ctx) => 0f;

        public void ReceiveSynergyBonus(float bonusRatio) { }
        public void ClearSynergyBonuses()                 { }
    }
}