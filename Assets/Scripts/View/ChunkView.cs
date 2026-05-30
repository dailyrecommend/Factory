using UnityEngine;

namespace PlanetCore
{
    public sealed class ChunkView : MonoBehaviour
    {
        private IGameConfig _config;
        private Chunk       _chunk;

        public void Init(Chunk chunk, IGameConfig config)
        {
            _chunk  = chunk;
            _config = config;

            _chunk.OnActiveChanged += OnActiveChanged;
            Render();
        }

        private void OnDestroy()
        {
            if (_chunk != null)
                _chunk.OnActiveChanged -= OnActiveChanged;
        }

        private void OnActiveChanged(bool isActive)
        {
            gameObject.SetActive(isActive);

            if (isActive)
                RestoreStructureAnimators();
        }

        public void Refresh()
        {
            gameObject.SetActive(_chunk.IsActive);
            if (_chunk.IsActive)
                RestoreStructureAnimators();
        }

        private void RestoreStructureAnimators()
        {
            foreach (var animator in GetComponentsInChildren<StructureAnimator>())
                animator.RestoreState();
        }

        private void Render()
        {
            foreach (var tile in _chunk.AllTiles())
            {
                var prefab = Resources.Load<GameObject>(tile.Definition.PrefabPath);

                if (prefab == null)
                {
                    Debug.LogError($"[ChunkView] Prefab not found: {tile.Definition.PrefabPath}");
                    continue;
                }

                var go = Instantiate(prefab, transform);
                go.transform.localPosition = TileToWorld(tile.WorldX, tile.WorldY);
                go.name  = $"Tile_{tile.WorldX}_{tile.WorldY}";
                go.layer = LayerMask.NameToLayer("Tile");

                var tileRef = go.AddComponent<TileRef>();
                tileRef.Tile  = tile;
                tileRef.Chunk = _chunk;
            }
        }

        private Vector3 TileToWorld(int worldX, int worldY)
            => new Vector3(worldX * _config.TileSize, 0f, worldY * _config.TileSize);
    }
}