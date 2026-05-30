using System;
using System.Collections.Generic;

namespace PlanetCore
{
    public sealed class Chunk
    {
        public int ChunkX { get; }
        public int ChunkY { get; }

        public event Action<bool> OnActiveChanged;

        private ChunkState _state = ChunkState.Active;
        public ChunkState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;

                bool isActive = IsActive;
                OnActiveChanged?.Invoke(isActive);

                for (int x = 0; x < _size; x++)
                for (int y = 0; y < _size; y++)
                {
                    var structure = _tiles[x, y]?.PlacedStructure;
                    if (structure == null) continue;

                    if (isActive)
                        structure.OnChunkActivated();
                    else
                        structure.OnChunkDeactivated();
                }
            }
        }

        public bool IsActive => _state == ChunkState.Active;

        private readonly TileData[,] _tiles;
        private readonly int         _size;

        public Chunk(int chunkX, int chunkY, int size)
        {
            ChunkX = chunkX;
            ChunkY = chunkY;
            _size  = size;
            _tiles = new TileData[size, size];
        }

        public TileData GetTile(int localX, int localY) => _tiles[localX, localY];

        public void SetTile(int localX, int localY, TileData tile)
            => _tiles[localX, localY] = tile;

        public IEnumerable<TileData> AllTiles()
        {
            for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
                yield return _tiles[x, y];
        }
    }
}