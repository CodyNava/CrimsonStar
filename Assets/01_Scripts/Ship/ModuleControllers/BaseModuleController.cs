using System;
using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.Modules;
using UnityEngine;
using UnityEngine.UIElements;

namespace _01_Scripts.Ship.ModuleControllers
{
    public abstract class BaseModuleController : MonoBehaviour
    {
        [SerializeField] private BaseModuleObject _moduleObject;
        public BaseModuleObject ModuleObject => _moduleObject;
        private bool _isCombatActive;
        public bool IsCombatActive => _isCombatActive;

        public void Awake()
        {
            _isCombatActive = false;
            Combat_GameState.onEnterState += OnEnterCombatState;
            Combat_GameState.onExitState += OnExitCombatState;
        }

        public void OnDestroy()
        {
            Combat_GameState.onEnterState -= OnEnterCombatState;
            Combat_GameState.onExitState -= OnExitCombatState;
        }

        protected virtual void OnExitCombatState()
        {
            _isCombatActive = false;
            Debug.Log("Set IsCombatActive: False");
        }
        
        protected virtual void OnEnterCombatState(GameStateController obj)
        {
            _isCombatActive = true;
            Debug.Log("Set IsCombatActive: True");
        }
    }
}