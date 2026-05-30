using System;
using System.Collections.Generic;

namespace PlanetCore
{
    // Manages all unlocked chunks and provides world-coordinate tile access.
    // Implements IWorldReader for read-only access by modules and resolvers.
    public sealed class WorldMap : IWorldReader
    {
        public int    Seed       { get; }
        public string PlanetName { get; }

        private readonly Dictionary<(int, int), Chunk> _chunks = new();
        private readonly ChunkGenerator                _generator;
        private readonly IGameConfig                   _config;
        private readonly DataRegistry                  _registry;

        public WorldMap(int seed, ChunkGenerator generator,
                        IGameConfig config, DataRegistry registry)
        {
            Seed       = seed;
            PlanetName = $"Planet-{Convert.ToString(seed, 16).ToUpper()}";
            _generator = generator;
            _config    = config;
            _registry  = registry;

            var start = UnlockChunk(0, 0);
            PlaceBasecamp(start);
        }

        // ── Chunk ──────────────────────────────────────────────────────────

        public bool IsChunkUnlocked(int cx, int cy)
            => _chunks.ContainsKey((cx, cy));

        public bool TryGetChunk(int cx, int cy, out Chunk chunk)
            => _chunks.TryGetValue((cx, cy), out chunk);

        public Chunk UnlockChunk(int chunkX, int chunkY)
        {
            var key = (chunkX, chunkY);
            if (_chunks.ContainsKey(key))
                throw new InvalidOperationException(
                    $"Chunk ({chunkX},{chunkY}) is already unlocked.");

            var chunk = _generator.Generate(chunkX, chunkY);
            _chunks[key] = chunk;
            return chunk;
        }

        public IEnumerable<(int cx, int cy)> GetExpandableCandidates()
        {
            var seen   = new HashSet<(int, int)>();
            var deltas = new[] { (0, 1), (0, -1), (1, 0), (-1, 0) };

            foreach (var kvp in _chunks)
            {
                // Only active chunks can expand
                if (!kvp.Value.IsActive) continue;

                foreach (var (dx, dy) in deltas)
                {
                    var candidate = (kvp.Key.Item1 + dx, kvp.Key.Item2 + dy);
                    if (!_chunks.ContainsKey(candidate) && seen.Add(candidate))
                        yield return candidate;
                }
            }
        }

        public IEnumerable<Chunk>    AllUnlockedChunks() => _chunks.Values;

        // ── Tile ───────────────────────────────────────────────────────────

        public bool TryGetTile(int worldX, int worldY, out TileData tile)
        {
            WorldToLocal(worldX, worldY, out int cx, out int cy, out int lx, out int ly);
            if (!_chunks.TryGetValue((cx, cy), out var chunk))
            { tile = null; return false; }
            tile = chunk.GetTile(lx, ly);
            return true;
        }

        public IEnumerable<TileData> AllOccupiedTiles()
        {
            foreach (var chunk in _chunks.Values)
            {
                // Skip inactive chunks — their structures should not tick
                if (!chunk.IsActive) continue;

                foreach (var tile in chunk.AllTiles())
                    if (!tile.IsEmpty)
                        yield return tile;
            }
        }

        // ── IWorldReader ───────────────────────────────────────────────────

        bool         IWorldReader.IsTileUnlocked(int wx, int wy) => TryGetTile(wx, wy, out _);
        TileData     IWorldReader.GetTile(int wx, int wy)
            => TryGetTile(wx, wy, out var t) ? t : null;
        IStructureComponent IWorldReader.GetBuildingAt(int wx, int wy)
            => TryGetTile(wx, wy, out var t) ? t.PlacedStructure : null;

        // ── Reset ──────────────────────────────────────────────────────────

        public void HardReset()
        {
            _chunks.Clear();
            var start = UnlockChunk(0, 0);
            PlaceBasecamp(start);
        }

        // ── Helpers ────────────────────────────────────────────────────────

        private void PlaceBasecamp(Chunk chunk)
        {
            // Basecamp module is attached to a prefab and instantiated by WorldRenderer.
            // WorldMap only reserves the tile — the actual IBuildingModule reference
            // is set by PlacementService after the prefab is instantiated.
            _ = chunk; // reservation noted; renderer handles instantiation
        }

        private void WorldToLocal(int wx, int wy,
            out int cx, out int cy, out int lx, out int ly)
        {
            int size = _config.ChunkSize;
            cx = FloorDiv(wx, size);
            cy = FloorDiv(wy, size);
            lx = wx - cx * size;
            ly = wy - cy * size;
        }

        private static int FloorDiv(int a, int b)
            => a / b - (a % b != 0 && (a ^ b) < 0 ? 1 : 0);
    }
}