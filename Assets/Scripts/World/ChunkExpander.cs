using System.Collections.Generic;
using UnityEngine;

namespace PlanetCore
{
    public sealed class ChunkExpander
    {
        private readonly WorldMap _world;

        public ChunkExpander(WorldMap world) => _world = world;

        // If there are inactive chunks, reactivate a random one.
        // Otherwise unlock a new adjacent chunk.
        public Chunk UnlockRandom()
        {
            // First check for inactive chunks to reactivate
            var inactive = new List<Chunk>();
            foreach (var chunk in _world.AllUnlockedChunks())
                if (!chunk.IsActive)
                    inactive.Add(chunk);

            if (inactive.Count > 0)
            {
                var chunk = inactive[Random.Range(0, inactive.Count)];
                Activate(chunk);
                Debug.Log($"[ChunkExpander] Reactivated chunk ({chunk.ChunkX},{chunk.ChunkY})");
                return chunk;
            }

            // No inactive chunks — unlock a new one
            var candidates = new List<(int cx, int cy)>(_world.GetExpandableCandidates());

            if (candidates.Count == 0)
            {
                Debug.Log("[ChunkExpander] No expandable candidates.");
                return null;
            }

            var (cx, cy) = candidates[Random.Range(0, candidates.Count)];
            var newChunk = _world.UnlockChunk(cx, cy);
            Debug.Log($"[ChunkExpander] Unlocked new chunk ({cx},{cy})");
            return newChunk;
        }

        public void Deactivate(Chunk chunk)
        {
            if (chunk == null) return;
            chunk.State = ChunkState.Inactive;
            Debug.Log($"[ChunkExpander] Deactivated chunk ({chunk.ChunkX},{chunk.ChunkY})");
        }

        public void Activate(Chunk chunk)
        {
            if (chunk == null) return;
            chunk.State = ChunkState.Active;
            Debug.Log($"[ChunkExpander] Activated chunk ({chunk.ChunkX},{chunk.ChunkY})");
        }
    }
}