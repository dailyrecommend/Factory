using System.Collections.Generic;

namespace PlanetCore
{
    public sealed class Chunk
    {
        public int        ChunkX { get; }
        public int        ChunkY { get; }
        public ChunkState State  { get; set; } = ChunkState.Active;

        private readonly TileData[,] _tiles;
        private readonly int         _size;

        public Chunk(int chunkX, int chunkY, int size)
        {
            ChunkX = chunkX;
            ChunkY = chunkY;
            _size  = size;
            _tiles = new TileData[size, size];
        }

        public bool IsActive => State == ChunkState.Active;

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