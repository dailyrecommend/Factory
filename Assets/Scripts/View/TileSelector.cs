using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetCore
{
    public sealed class TileSelector : MonoBehaviour
    {
        [SerializeField] private Camera           _camera;
        [SerializeField] private PlacementManager _placementManager;
        [SerializeField] private LayerMask        _tileLayer;

        private void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray      = _camera.ScreenPointToRay(mousePos);
            
            
            
            if (!Physics.Raycast(ray, out var hit, 200f, _tileLayer))
            {
                _placementManager.OnTileHoverExit();
                return;
            }

            var tileRef = hit.collider.GetComponent<TileRef>();
            if (tileRef == null) return;
            
            if (tileRef.Chunk != null && !tileRef.Chunk.IsActive) return;

            // Always call OnTileHover every frame so preview material stays current
            _placementManager.OnTileHover(tileRef.Tile);

            if (Mouse.current.leftButton.wasPressedThisFrame)
                _placementManager.OnTileClicked(tileRef.Tile);
        }
    }
}