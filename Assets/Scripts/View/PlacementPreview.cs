using UnityEngine;

namespace PlanetCore
{
    public sealed class PlacementPreview : MonoBehaviour
    {
        [SerializeField] private Material _validMaterial;
        [SerializeField] private Material _invalidMaterial;

        private GameObject   _previewGo;
        private GameObject   _currentPrefab;
        private Renderer[]   _renderers;

        // Demolish mode — track actual structure to restore materials
        private GameObject   _demolishTarget;
        private Renderer[]   _demolishRenderers;
        private Material[][] _originalMaterials;

        public void Show(GameObject prefab, Vector3 position, bool isValid)
        {
            // Restore demolish target if switching from demolish mode
            RestoreDemolishTarget();

            if (_currentPrefab != prefab)
            {
                CreatePreview(prefab);
                _currentPrefab = prefab;
            }

            _previewGo.transform.position = position;
            _previewGo.SetActive(true);

            ApplyMaterial(_renderers, isValid ? _validMaterial : _invalidMaterial);
        }

        public void ShowDemolish(GameObject target, Vector3 position)
        {
            // Hide ghost preview
            if (_previewGo != null)
                _previewGo.SetActive(false);

            // Restore previous demolish target first
            if (_demolishTarget != target)
            {
                RestoreDemolishTarget();
                SaveAndApplyDemolish(target);
            }
        }

        public void Hide()
        {
            if (_previewGo != null)
                _previewGo.SetActive(false);

            RestoreDemolishTarget();
        }

        private void SaveAndApplyDemolish(GameObject target)
        {
            _demolishTarget    = target;
            _demolishRenderers = target.GetComponentsInChildren<Renderer>();
            _originalMaterials = new Material[_demolishRenderers.Length][];

            for (int i = 0; i < _demolishRenderers.Length; i++)
            {
                // Save original materials
                _originalMaterials[i] = _demolishRenderers[i].materials;

                // Apply invalid material to all slots
                var mats = new Material[_demolishRenderers[i].materials.Length];
                for (int j = 0; j < mats.Length; j++)
                    mats[j] = _invalidMaterial;
                _demolishRenderers[i].materials = mats;
            }
        }

        private void RestoreDemolishTarget()
        {
            if (_demolishTarget == null) return;

            for (int i = 0; i < _demolishRenderers.Length; i++)
            {
                if (_demolishRenderers[i] != null)
                    _demolishRenderers[i].materials = _originalMaterials[i];
            }

            _demolishTarget    = null;
            _demolishRenderers = null;
            _originalMaterials = null;
        }

        private void CreatePreview(GameObject prefab)
        {
            if (_previewGo != null)
                Destroy(_previewGo);

            _previewGo = Instantiate(prefab, transform);
            _previewGo.name = "Preview";

            foreach (var col in _previewGo.GetComponentsInChildren<Collider>())
                col.enabled = false;

            foreach (var mb in _previewGo.GetComponentsInChildren<MonoBehaviour>())
                mb.enabled = false;

            _renderers = _previewGo.GetComponentsInChildren<Renderer>();
        }

        private static void ApplyMaterial(Renderer[] renderers, Material mat)
        {
            foreach (var r in renderers)
            {
                var mats = new Material[r.materials.Length];
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = mat;
                r.materials = mats;
            }
        }

        private void OnDestroy()
        {
            RestoreDemolishTarget();
            if (_previewGo != null)
                Destroy(_previewGo);
        }
    }
}