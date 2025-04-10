using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.Modules;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public abstract class BaseModuleController : MonoBehaviour
    {
        [SerializeField] private BaseModuleObject _moduleObject;
        public BaseModuleObject ModuleObject => _moduleObject;
        private bool _isCombatActive;
        public bool IsCombatActive => _isCombatActive;
        private float currentHp;

        protected BridgeController BridgeController;

        public virtual void Awake()
        {
            _isCombatActive = false;
            Combat_GameState.onEnterState += OnEnterCombatState;
            Combat_GameState.onExitState += OnExitCombatState;
            currentHp = _moduleObject._health;
        }
        

        public virtual void Start()
        { }

        public void Init(BridgeController bridgeController)
        {
            BridgeController = bridgeController;
            BridgeController.AddModule(this);
        }

        public virtual void OnDestroy()
        {
            Combat_GameState.onEnterState -= OnEnterCombatState;
            Combat_GameState.onExitState -= OnExitCombatState;
            if (BridgeController != null)
            {
                BridgeController.RemoveModule(this);
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
            Debug.Log("Hitted");
            if (other.transform.root == transform.root) return;
            

            if (other.transform.TryGetComponent(out Projectile projectile))
            {
                print("hit");

                float updatedHP = Mathf.Max(currentHp - projectile._BaseProjectileObject.Damage, 0f);
                float deltaHP = updatedHP - currentHp;
                BridgeController.ModifyCurrentHp(deltaHP);
                currentHp = updatedHP;
                
                
                if (currentHp <= 0)
                {
                    OnModuleDestroyed();
                }
            }
        }

        protected virtual void OnModuleDestroyed()
        {
            Destroy(this.gameObject);
            print("Death");
        }
    }
}