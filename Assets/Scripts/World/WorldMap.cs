using System;
using System.Collections.Generic;

namespace PlanetCore
{
    public class WorldMap : IWorldContext
    {
        public int    Seed       { get; }
        public string PlanetName { get; }

        private readonly Dictionary<(int, int), Chunk> _chunks
            = new Dictionary<(int, int), Chunk>();

        private readonly ChunkGenerator _generator;

        public WorldMap(int seed, ChunkGenerator generator)
        {
            Seed        = seed;
            PlanetName  = $"Planet {Convert.ToString(seed, 16).ToUpper()}";
            _generator  = generator;

            var start = UnlockChunk(0, 0);
            PlaceBasecamp(start);
        }

        public bool IsChunkUnlocked(int cx, int cy)
            => _chunks.ContainsKey((cx, cy));

        public IEnumerable<(int cx, int cy)> GetExpandableCandidates()
        {
            var seen   = new HashSet<(int, int)>();
            var deltas = new[] { (0,1),(0,-1),(1,0),(-1,0) };

            foreach (var key in _chunks.Keys)
            foreach (var (dx, dy) in deltas)
            {
                var candidate = (key.Item1 + dx, key.Item2 + dy);
                if (!_chunks.ContainsKey(candidate) && seen.Add(candidate))
                    yield return candidate;
            }
        }

        public Chunk UnlockChunk(int chunkX, int chunkY)
        {
            var key = (chunkX, chunkY);
            if (_chunks.ContainsKey(key))
                throw new InvalidOperationException($"Chunk ({chunkX},{chunkY}) already unlocked.");

            var chunk = _generator.Generate(chunkX, chunkY);
            _chunks[key] = chunk;
            return chunk;
        }

        public bool TryGetChunk(int cx, int cy, out Chunk chunk)
            => _chunks.TryGetValue((cx, cy), out chunk);

        public bool TryGetTile(int worldX, int worldY, out Tile tile)
        {
            WorldToLocal(worldX, worldY, out int cx, out int cy, out int lx, out int ly);

            if (!_chunks.TryGetValue((cx, cy), out var chunk))
            {
                tile = null;
                return false;
            }
            tile = chunk.GetTile(lx, ly);
            return true;
        }

        bool IWorldContext.IsTileUnlocked(int wx, int wy)   => TryGetTile(wx, wy, out _);
        IBuildingBehavior IWorldContext.GetBuildingAt(int wx, int wy)
            => TryGetTile(wx, wy, out var t) ? t.PlacedBuilding : null;
        TerrainType IWorldContext.GetTerrainType(int wx, int wy)
            => TryGetTile(wx, wy, out var t) ? t.TerrainType : TerrainType.Plain;
        ResourceType IWorldContext.GetResourceType(int wx, int wy)
            => TryGetTile(wx, wy, out var t) ? t.ResourceType : ResourceType.None;

        public IEnumerable<Tile> AllOccupiedTiles()
        {
            foreach (var chunk in _chunks.Values)
            foreach (var tile in chunk.AllTiles())
                if (tile.PlacedBuilding != null)
                    yield return tile;
        }

        public IEnumerable<Chunk> AllUnlockedChunks() => _chunks.Values;

        public void HardReset()
        {
            _chunks.Clear();
            var start = UnlockChunk(0, 0);
            PlaceBasecamp(start);
        }

        private static void WorldToLocal(int wx, int wy,
            out int cx, out int cy, out int lx, out int ly)
        {
            int size = GameConstants.ChunkSize;
            cx = FloorDiv(wx, size);
            cy = FloorDiv(wy, size);
            lx = wx - cx * size;
            ly = wy - cy * size;
        }

        private static int FloorDiv(int a, int b)
            => a / b - (a % b != 0 && (a ^ b) < 0 ? 1 : 0);

        private static void PlaceBasecamp(Chunk chunk)
        {
            var tile = chunk.GetTile(GameConstants.BasecampLocalX, GameConstants.BasecampLocalY);
            tile.PlacedBuilding = new Basecamp { WorldPosition = (tile.WorldX, tile.WorldY) };
        }
    }
}
