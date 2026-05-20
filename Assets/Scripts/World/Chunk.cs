using System.Collections.Generic;

namespace PlanetCore
{
    public class Chunk
    {
        public int ChunkX { get; }
        public int ChunkY { get; }

        private readonly Tile[,] _tiles =
            new Tile[GameConstants.ChunkSize, GameConstants.ChunkSize];

        public Chunk(int chunkX, int chunkY)
        {
            ChunkX = chunkX;
            ChunkY = chunkY;
        }

        public Tile GetTile(int localX, int localY) => _tiles[localX, localY];

        public void SetTile(int localX, int localY, Tile tile)
            => _tiles[localX, localY] = tile;

        public IEnumerable<Tile> AllTiles()
        {
            for (int x = 0; x < GameConstants.ChunkSize; x++)
            for (int y = 0; y < GameConstants.ChunkSize; y++)
                yield return _tiles[x, y];
        }
    }
}
