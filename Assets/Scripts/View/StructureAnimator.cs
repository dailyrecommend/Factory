using UnityEngine;

namespace PlanetCore
{
    public sealed class StructureAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private static readonly int ParamOnPlaced  = Animator.StringToHash("OnPlaced");
        private static readonly int ParamIsWorking = Animator.StringToHash("IsWorking");

        private IStructureComponent _structure;

        private void Awake()
        {
            _structure = GetComponent<IStructureComponent>();

            if (_structure == null)
                Debug.LogError($"[StructureAnimator] No IStructureComponent on {gameObject.name}");

            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        public void PlayPlaceAnimation()
        {
            if (_animator == null) return;
            _animator.SetTrigger(ParamOnPlaced);
        }

        public void RestoreState()
        {
            if (_animator == null || _structure == null) return;
            _animator.SetBool(ParamIsWorking, _structure.IsOperational);
        }

        private void Update()
        {
            if (_animator == null || _structure == null) return;
            _animator.SetBool(ParamIsWorking, _structure.IsOperational);
        }
    }
}