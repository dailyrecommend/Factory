using UnityEngine;

namespace PlanetCore
{
    // Attached to structure prefabs alongside IStructureComponent.
    // Watches IsOperational and drives the Animator accordingly.
    // Components know nothing about animation.
    public sealed class StructureAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        // Animator parameter names
        private static readonly int ParamOnPlaced  = Animator.StringToHash("OnPlaced");
        private static readonly int ParamIsWorking = Animator.StringToHash("IsWorking");

        private IStructureComponent _structure;

        private void Awake()
        {
            _structure = GetComponent<IStructureComponent>();

            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            // Use unscaled time so we can control pause manually
            if (_animator != null)
                _animator.updateMode = AnimatorUpdateMode.Normal;
        }

        // Called by PlacementService after OnPlaced
        public void PlayPlaceAnimation()
        {
            if (_animator == null) return;
            _animator.SetTrigger(ParamOnPlaced);
        }

        private void Update()
        {
            if (_animator == null || _structure == null) return;
            _animator.SetBool(ParamIsWorking, _structure.IsOperational);
        }
    }
}