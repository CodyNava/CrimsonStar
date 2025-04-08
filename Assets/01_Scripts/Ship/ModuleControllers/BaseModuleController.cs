using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class BaseModuleController : MonoBehaviour
    {
        private bool _isCombatActive;
        
        public void Awake()
        {
            Combat_GameState.onEnterState += OnEnterCombatState;
            Combat_GameState.onExitState += OnExitCombatState;
        }

        public void OnDestroy()
        {
            Combat_GameState.onEnterState -= OnEnterCombatState;
            Combat_GameState.onExitState -= OnExitCombatState;
        }

        private void OnExitCombatState()
        {
            _isCombatActive = false;
        }
        
        private void OnEnterCombatState(GameStateController obj)
        {
            _isCombatActive = true;
        }
    }
}