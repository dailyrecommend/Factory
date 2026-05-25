using System.Collections.Generic;
using UnityEngine;

namespace PlanetCore
{
    public sealed class WorldRenderer : MonoBehaviour
    {
        private WorldMap    _world;
        private IGameConfig _config;

        private readonly Dictionary<(int, int), ChunkView> _chunkViews = new();

        public void Init(WorldMap world, IGameConfig config)
        {
            _world  = world;
            _config = config;

            foreach (var chunk in _world.AllUnlockedChunks())
                SpawnChunkView(chunk);
        }

        public void SpawnChunkView(Chunk chunk)
        {
            var go = new GameObject($"Chunk_{chunk.ChunkX}_{chunk.ChunkY}");
            go.transform.SetParent(transform);

            var view = go.AddComponent<ChunkView>();
            view.Init(chunk, _config);

            _chunkViews[(chunk.ChunkX, chunk.ChunkY)] = view;
        }

        // Called when chunk state changes — refreshes tile colors
        public void RefreshChunkView(Chunk chunk)
        {
            if (_chunkViews.TryGetValue((chunk.ChunkX, chunk.ChunkY), out var view))
                view.Refresh();
        }
    }
}