using System;
using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.Modules;
using Unity.VisualScripting;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class BaseModuleController : MonoBehaviour
    {
        [SerializeField] private BaseModuleObject _moduleObject;
        public BaseModuleObject ModuleObject => _moduleObject;
        private bool _isCombatActive;
        public bool IsCombatActive => _isCombatActive;
        private float currentHp;

        public void Awake()
        {
            _isCombatActive = false;
            Combat_GameState.onEnterState += OnEnterCombatState;
            Combat_GameState.onExitState += OnExitCombatState;
            currentHp = _moduleObject._health;
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.TryGetComponent(out Projectile projectile))
            {
                currentHp -= projectile._BaseProjectileObject.Damage;
                if (currentHp <= 0)
                {
                    OnModuleDestroyed();
                }
            }
        }

        private void OnModuleDestroyed()
        {
            print("lol");
        }
    }
}