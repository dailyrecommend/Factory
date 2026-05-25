using UnityEngine;

namespace PlanetCore
{
    // Attached to every tile GameObject by ChunkView.
    // Allows raycasting code to identify a tile without parsing object names.
    public sealed class TileRef : MonoBehaviour
    {
        public TileData Tile { get; set; }
        public Chunk Chunk { get; set; }
    }
}