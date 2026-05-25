using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetCore
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float  _dragSpeed  = 0.5f;
        [SerializeField] private float  _zoomSpeed  = 2f;
        [SerializeField] private float  _minOrthoSize = 3f;
        [SerializeField] private float  _maxOrthoSize = 20f;

        private WorldMap    _world;
        private IGameConfig _config;
        private Vector2     _lastMousePos;
        private bool        _isDragging;

        private float ZoomRatio => (_camera.orthographicSize - _minOrthoSize)
                                 / (_maxOrthoSize - _minOrthoSize);

        public void Init(WorldMap world, IGameConfig config)
        {
            _world  = world;
            _config = config;
        }

        private void Update()
        {
            HandleZoom();
            HandleDrag();
        }

        private void HandleZoom()
        {
            var scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Approximately(scroll, 0f)) return;

            _camera.orthographicSize = Mathf.Clamp(
                _camera.orthographicSize - scroll * _zoomSpeed * Time.deltaTime,
                _minOrthoSize,
                _maxOrthoSize);
        }

        private void HandleDrag()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.rightButton.wasPressedThisFrame)
            {
                _isDragging   = true;
                _lastMousePos = mouse.position.ReadValue();
            }

            if (mouse.rightButton.wasReleasedThisFrame)
                _isDragging = false;

            if (!_isDragging) return;

            var currentMousePos = mouse.position.ReadValue();
            var delta           = currentMousePos - _lastMousePos;
            _lastMousePos       = currentMousePos;

            // Scale drag speed with zoom so panning feels consistent
            float speed = _dragSpeed * Mathf.Lerp(1f, 3f, ZoomRatio);
            var   forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            var   right   = Vector3.ProjectOnPlane(transform.right,   Vector3.up).normalized;
            var move = (-right * delta.x + -forward * delta.y) * speed * Time.unscaledDeltaTime;

            transform.position = Clamp(transform.position + move);
        }

        private Vector3 Clamp(Vector3 pos)
        {
            var (minX, maxX, minZ, maxZ) = GetBounds();
            return new Vector3(
                Mathf.Clamp(pos.x, minX, maxX),
                pos.y,
                Mathf.Clamp(pos.z, minZ, maxZ));
        }

        private (float minX, float maxX, float minZ, float maxZ) GetBounds()
        {
            int   size      = _config.ChunkSize;
            float tileSize  = _config.TileSize;
            float chunkSize = size * tileSize;

            int minCx = int.MaxValue, maxCx = int.MinValue;
            int minCy = int.MaxValue, maxCy = int.MinValue;

            foreach (var chunk in _world.AllUnlockedChunks())
            {
                if (chunk.ChunkX < minCx) minCx = chunk.ChunkX;
                if (chunk.ChunkX > maxCx) maxCx = chunk.ChunkX;
                if (chunk.ChunkY < minCy) minCy = chunk.ChunkY;
                if (chunk.ChunkY > maxCy) maxCy = chunk.ChunkY;
            }

            float minX = (minCx - 2) * chunkSize;
            float maxX = (maxCx + 2) * chunkSize;
            float minZ = (minCy - 2) * chunkSize;
            float maxZ = (maxCy + 2) * chunkSize;

            return (minX, maxX, minZ, maxZ);
        }
    }
}