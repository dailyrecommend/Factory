using UnityEngine;

namespace PlanetCore
{
    public sealed class ChunkView : MonoBehaviour
    {
        private IGameConfig _config;
        private Chunk       _chunk;

        private static readonly Color ColorPlain    = new Color(0.55f, 0.76f, 0.45f);
        private static readonly Color ColorDeposit  = new Color(0.40f, 0.28f, 0.18f);
        private static readonly Color ColorInactive = new Color(0.4f,  0.4f,  0.4f);

        public void Init(Chunk chunk, IGameConfig config)
        {
            _chunk  = chunk;
            _config = config;
            Render();
        }

        // Called when chunk state changes (active ↔ inactive)
        public void Refresh()
        {
            foreach (Transform child in transform)
            {
                var tileRef = child.GetComponent<TileRef>();
                if (tileRef == null) continue;

                var renderer = child.GetComponentInChildren<Renderer>();
                if (renderer == null) continue;

                renderer.material.color = _chunk.IsActive
                    ? TileColor(tileRef.Tile)
                    : ColorInactive;
            }
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

        private static Color TileColor(TileData tile)
        {
            if (tile.HasTag("mineable")) return ColorDeposit;
            return ColorPlain;
        }
    }
}