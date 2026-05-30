using System;

namespace PlanetCore
{
    public sealed class ChunkGenerator
    {
        private readonly int          _worldSeed;
        private readonly DataRegistry _registry;
        private int                   _minDepositCount;

        private const string PlainId   = "plain";
        private const string DepositId = "resource_deposit";

        public ChunkGenerator(int worldSeed, DataRegistry registry, int minDepositCount = 0)
        {
            _worldSeed       = worldSeed;
            _registry        = registry;
            _minDepositCount = minDepositCount;
        }

        public void IncreaseMinDeposits(int delta)
            => _minDepositCount = Math.Max(0, _minDepositCount + delta);

        public Chunk Generate(int chunkX, int chunkY)
        {
            var cfg        = _registry.Config;
            int size       = cfg.ChunkSize;
            var chunk      = new Chunk(chunkX, chunkY, size);
            var rng        = MakeRng(chunkX, chunkY);
            var plainDef   = _registry.GetTile(PlainId);
            var depositDef = _registry.GetTile(DepositId);
            int depositCount = 0;

            for (int lx = 0; lx < size; lx++)
            for (int ly = 0; ly < size; ly++)
            {
                int  wx        = chunkX * size + lx;
                int  wy        = chunkY * size + ly;
                bool isDeposit = rng.NextDouble() < cfg.DepositChance;
                if (isDeposit) depositCount++;

                chunk.SetTile(lx, ly, new TileData(wx, wy,
                    isDeposit ? depositDef : plainDef, chunk));
            }

            for (int lx = 0; lx < size && depositCount < _minDepositCount; lx++)
            for (int ly = 0; ly < size && depositCount < _minDepositCount; ly++)
            {
                var tile = chunk.GetTile(lx, ly);
                if (!tile.HasTag("mineable"))
                {
                    chunk.SetTile(lx, ly, new TileData(tile.WorldX, tile.WorldY, depositDef, chunk));
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