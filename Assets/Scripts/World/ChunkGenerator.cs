using System;

namespace PlanetCore
{
    public class ChunkGenerator
    {
        private readonly int _worldSeed;
        private int _minDepositCount;

        public ChunkGenerator(int worldSeed, int minDepositCount = 0)
        {
            _worldSeed        = worldSeed;
            _minDepositCount  = minDepositCount;
        }

        public void IncreaseMinDeposits(int delta)
            => _minDepositCount = Math.Max(0, _minDepositCount + delta);

        public Chunk Generate(int chunkX, int chunkY)
        {
            var chunk = new Chunk(chunkX, chunkY);
            var rng   = MakeRng(chunkX, chunkY);
            int size  = GameConstants.ChunkSize;
            int depositCount = 0;

            for (int lx = 0; lx < size; lx++)
            for (int ly = 0; ly < size; ly++)
            {
                int  worldX    = chunkX * size + lx;
                int  worldY    = chunkY * size + ly;
                bool isDeposit = rng.NextDouble() < 0.25;
                if (isDeposit) depositCount++;

                chunk.SetTile(lx, ly, new Tile(
                    worldX, worldY,
                    isDeposit ? TerrainType.ResourceDeposit : TerrainType.Plain,
                    isDeposit ? ResourceType.Coal            : ResourceType.None));
            }

            for (int lx = 0; lx < size && depositCount < _minDepositCount; lx++)
            for (int ly = 0; ly < size && depositCount < _minDepositCount; ly++)
            {
                var tile = chunk.GetTile(lx, ly);
                if (tile.TerrainType == TerrainType.Plain)
                {
                    tile.TerrainType  = TerrainType.ResourceDeposit;
                    tile.ResourceType = ResourceType.Coal;
                    depositCount++;
                }
            }

            return chunk;
        }

        private Random MakeRng(int cx, int cy)
        {
            const int P1 = 1_000_003;
            const int P2 = 999_983;
            return new Random(_worldSeed ^ (cx * P1) ^ (cy * P2));
        }
    }
}
