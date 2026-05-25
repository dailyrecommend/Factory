using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetCore
{
    public sealed class PlacementManager : MonoBehaviour
    {
        [SerializeField] private PlacementPreview _preview;

        private PlacementService _placementService;
        private DataRegistry     _registry;
        private IGameConfig      _config;

        private string     _selectedStructureId;
        private GameObject _selectedPrefab;
        private bool       _isDemolishMode;

        private bool _isPlacing => _selectedStructureId != null && !_isDemolishMode;

        public void Init(PlacementService placementService, DataRegistry registry)
        {
            _placementService = placementService;
            _registry         = registry;
            _config           = registry.Config;
        }

        public void SelectStructure(string structureId)
        {
            if (!_registry.TryGetStructure(structureId, out var def))
            {
                Debug.LogWarning($"[PlacementManager] Unknown structureId: {structureId}");
                return;
            }

            // Selecting a structure exits demolish mode
            _isDemolishMode      = false;
            _selectedStructureId = structureId;
            _selectedPrefab      = Resources.Load<GameObject>(def.PrefabPath);

            if (_selectedPrefab == null)
            {
                Debug.LogError($"[PlacementManager] Prefab not found: {def.PrefabPath}");
                CancelAll();
            }
        }

        public void CancelAll()
        {
            _selectedStructureId = null;
            _selectedPrefab      = null;
            _isDemolishMode      = false;
            _preview.Hide();
        }

        // Called by TileSelector every frame
        public void OnTileHover(TileData tile)
        {
            if (_isPlacing)
            {
                bool    isValid  = CanPlace(tile);
                Vector3 position = TileToWorld(tile);
                _preview.Show(_selectedPrefab, position, isValid);
                return;
            }

            if (_isDemolishMode)
            {
                // Show the placed structure with invalid material as demolish indicator
                if (!tile.IsEmpty && tile.PlacedStructure.StructureId != "basecamp")
                {
                    var structure = tile.PlacedStructure as MonoBehaviour;
                    if (structure != null)
                        _preview.ShowDemolish(structure.gameObject, TileToWorld(tile));
                }
                else
                {
                    _preview.Hide();
                }
                return;
            }

            _preview.Hide();
        }

        public void OnTileHoverExit()
            => _preview.Hide();

        public void OnTileClicked(TileData tile)
        {
            if (_isDemolishMode)
            {
                TryDemolish(tile);
                return;
            }

            if (_isPlacing)
                TryPlace(tile);
        }

        private void TryPlace(TileData tile)
        {
            var result = _placementService.TryPlace(
                _selectedStructureId, tile.WorldX, tile.WorldY);

            if (result == PlacementResult.Success)
                Debug.Log($"[PlacementManager] Placed '{_selectedStructureId}' at ({tile.WorldX},{tile.WorldY})");
            else
                Debug.Log($"[PlacementManager] Placement failed: {result}");
        }

        private void TryDemolish(TileData tile)
        {
            if (tile.IsEmpty)
            {
                Debug.Log("[PlacementManager] No structure to demolish.");
                return;
            }

            bool success = _placementService.TryDemolish(tile.WorldX, tile.WorldY);
            Debug.Log(success
                ? $"[PlacementManager] Demolished at ({tile.WorldX},{tile.WorldY})"
                : $"[PlacementManager] Cannot demolish at ({tile.WorldX},{tile.WorldY})");
        }

        private bool CanPlace(TileData tile)
        {
            if (!tile.IsEmpty) return false;
            if (!_registry.TryGetStructure(_selectedStructureId, out var def)) return false;
            if (def.HasTileRequirement && !tile.HasTag(def.RequiredTileTag)) return false;
            return true;
        }

        private Vector3 TileToWorld(TileData tile)
            => new Vector3(
                tile.WorldX * _config.TileSize,
                0f,
                tile.WorldY * _config.TileSize);

        private void Update()
        {
            var keyboard = Keyboard.current;

            if (keyboard.qKey.wasPressedThisFrame)
                SelectStructure("generator");

            if (keyboard.wKey.wasPressedThisFrame)
                SelectStructure("extractor");

            if (keyboard.eKey.wasPressedThisFrame)
                SelectStructure("energy_storage");
            
            if (keyboard.fKey.wasPressedThisFrame) 
                SelectStructure("broker");

            if (keyboard.rKey.wasPressedThisFrame)
            {
                // Toggle demolish mode
                _isDemolishMode = !_isDemolishMode;
                if (_isDemolishMode)
                {
                    _selectedStructureId = null;
                    _selectedPrefab      = null;
                    _preview.Hide();
                    Debug.Log("[PlacementManager] Demolish mode ON");
                }
                else
                {
                    Debug.Log("[PlacementManager] Demolish mode OFF");
                }
            }

            if (keyboard.escapeKey.wasPressedThisFrame)
                CancelAll();
        }
    }
}