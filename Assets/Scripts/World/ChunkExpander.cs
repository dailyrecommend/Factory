using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetCore
{
    // Manages chunk unlocking and activation state.
    // Each BrokerComponent holds a reference to its unlocked chunk.
    public sealed class ChunkExpander
    {
        private readonly WorldMap _world;

        public ChunkExpander(WorldMap world) => _world = world;

        // Unlocks a random adjacent inactive chunk.
        // Returns the chunk, or null if no candidates exist.
        public Chunk UnlockRandom()
        {
            var candidates = new List<(int cx, int cy)>(_world.GetExpandableCandidates());

            if (candidates.Count == 0)
            {
                Debug.Log("[ChunkExpander] No expandable candidates.");
                return null;
            }

            var (cx, cy) = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            var chunk    = _world.UnlockChunk(cx, cy);

            Debug.Log($"[ChunkExpander] Unlocked chunk ({cx},{cy})");
            return chunk;
        }

        // Deactivates a chunk — tiles become unusable, shown in grey.
        public void Deactivate(Chunk chunk)
        {
            if (chunk == null) return;
            chunk.State = ChunkState.Inactive;
            Debug.Log($"[ChunkExpander] Deactivated chunk ({chunk.ChunkX},{chunk.ChunkY})");
        }

        // Reactivates a previously deactivated chunk.
        public void Activate(Chunk chunk)
        {
            if (chunk == null) return;
            chunk.State = ChunkState.Active;
            Debug.Log($"[ChunkExpander] Activated chunk ({chunk.ChunkX},{chunk.ChunkY})");
        }
    }
}