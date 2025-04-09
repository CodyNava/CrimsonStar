using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.Modules;
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

        protected BridgeController BridgeController;

        public void Awake()
        {
            _isCombatActive = false;
            Combat_GameState.onEnterState += OnEnterCombatState;
            Combat_GameState.onExitState += OnExitCombatState;
            currentHp = _moduleObject._health;
        }

        public void Init(BridgeController bridgeController)
        {
            BridgeController = bridgeController;
            BridgeController.AddModule(_moduleObject);
        }

        public void OnDestroy()
        {
            Combat_GameState.onEnterState -= OnEnterCombatState;
            Combat_GameState.onExitState -= OnExitCombatState;
            if (BridgeController != null)
            {
                BridgeController.RemoveModule(_moduleObject);
            }
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.root == transform.root) return;

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