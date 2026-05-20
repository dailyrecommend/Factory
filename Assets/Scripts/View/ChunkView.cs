using UnityEngine;

namespace PlanetCore
{
    /// <summary>
    /// Spawns and manages the visual tile GameObjects for one chunk.
    /// Attach this to a ChunkView prefab or empty GameObject.
    /// </summary>
    public class ChunkView : MonoBehaviour
    {
        private GameObject[,] _tileObjects = new GameObject[GameConstants.ChunkSize, GameConstants.ChunkSize];
        private Chunk _chunk;

        /// <summary>
        /// Spawns all 5x5 tiles for the given chunk.
        /// Called once by WorldRenderer when a chunk is unlocked.
        /// </summary>
        public void Initialise(Chunk chunk, GameObject tilePrefab, float tileSize)
        {
            _chunk = chunk;

            for (int lx = 0; lx < GameConstants.ChunkSize; lx++)
            for (int ly = 0; ly < GameConstants.ChunkSize; ly++)
            {
                var tile = chunk.GetTile(lx, ly);

                Vector3 worldPos = new Vector3(
                    tile.WorldX * tileSize,
                    0f,
                    tile.WorldY * tileSize);

                var obj = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                obj.name = $"Tile_{tile.WorldX}_{tile.WorldY}";

                _tileObjects[lx, ly] = obj;
            }
        }
    }
}
