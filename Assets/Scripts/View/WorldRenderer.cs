using UnityEngine;

namespace PlanetCore
{
    /// <summary>
    /// Reads the WorldMap and spawns ChunkView objects.
    /// Attach to a GameObject in the scene (e.g. "WorldRenderer").
    ///
    /// Setup in Inspector:
    ///   - Tile Prefab  : 타일 프리팹 드래그앤드롭
    ///   - Tile Size    : 타일 한 칸의 크기 (기본 1)
    /// </summary>
    public class WorldRenderer : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _tilePrefab;

        [Header("Settings")]
        [SerializeField] private float _tileSize = 1f;

        private void Start()
        {
            RenderStartingChunk();
        }

        /// <summary>
        /// 게임 시작 시 (0,0) 청크를 렌더링합니다.
        /// </summary>
        private void RenderStartingChunk()
        {
            if (_tilePrefab == null)
            {
                Debug.LogError("[WorldRenderer] Tile Prefab이 연결되지 않았습니다. Inspector를 확인하세요.");
                return;
            }

            var world = GameManager.Instance.World;

            if (!world.TryGetChunk(0, 0, out var startChunk))
            {
                Debug.LogError("[WorldRenderer] 시작 청크 (0,0)를 찾을 수 없습니다.");
                return;
            }

            SpawnChunk(startChunk);

            Debug.Log($"[WorldRenderer] 청크 (0,0) 렌더링 완료 — {GameConstants.ChunkSize}x{GameConstants.ChunkSize} 타일");
        }

        /// <summary>
        /// 청크 데이터를 받아 ChunkView를 생성하고 타일을 스폰합니다.
        /// 나중에 청크 확장 카드 사용 시 이 메서드를 재사용합니다.
        /// </summary>
        public ChunkView SpawnChunk(Chunk chunk)
        {
            var chunkRoot = new GameObject($"Chunk_{chunk.ChunkX}_{chunk.ChunkY}");
            chunkRoot.transform.SetParent(transform);

            var view = chunkRoot.AddComponent<ChunkView>();
            view.Initialise(chunk, _tilePrefab, _tileSize);

            return view;
        }
    }
}
