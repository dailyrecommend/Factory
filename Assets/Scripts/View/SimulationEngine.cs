using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetCore
{
    public sealed class SimulationEngine : MonoBehaviour
    {
        private WorldMap         _world;
        private EnergyPool       _energyPool;
        private SynergyResolver  _synergyResolver;
        private ComponentContext _componentCtx;
        private TurnManager      _turnManager;

        private bool  _isPaused     = false;
        private float _speedMultiplier = 1f;

        public void Init(
            WorldMap world,
            EnergyPool energyPool,
            SynergyResolver synergyResolver,
            ComponentContext componentCtx,
            TurnManager turnManager)
        {
            _world           = world;
            _energyPool      = energyPool;
            _synergyResolver = synergyResolver;
            _componentCtx    = componentCtx;
            _turnManager     = turnManager;
        }

        private void Update()
        {
            if (_turnManager.IsGameOver) return;

            _synergyResolver.ResolveSynergies();

            float totalEnergy = 0f;
            foreach (var tile in _world.AllOccupiedTiles())
            {
                var structure = tile.PlacedStructure;
                if (structure == null || !structure.IsOperational) continue;
                totalEnergy += structure.Tick(Time.deltaTime, _componentCtx);
            }

            if (totalEnergy > 0f)
                _energyPool.Add(totalEnergy);

            if (_turnManager.Tick(Time.deltaTime))
                ExecuteSettlement();
        }
        
        private void SetAnimatorsPaused(bool paused)
        {
            foreach (var animator in FindObjectsByType<Animator>(FindObjectsSortMode.None))
                animator.speed = paused ? 0f : _speedMultiplier;
        }

        private void ExecuteSettlement()
        {
            var result = _turnManager.ExecuteSettlement();

            if (result.QuotaMet)
                Debug.Log($"[Settlement] Day {result.DayNumber} — PASSED" +
                          $" | Delivered: {result.EnergyDelivered:F1}" +
                          $" | Quota: {result.Quota:F1}" +
                          $" | Credits: {result.CreditsEarned:F1}");
            else
                Debug.Log($"[Settlement] Day {result.DayNumber} — FAILED" +
                          $" | Shortfall: {result.EnergyShortfall:F1}");

            if (!result.QuotaMet) return;
            _turnManager.AdvanceToNextDay();
        }
    }
}