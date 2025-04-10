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

        protected ShipController _shipController;

        public virtual void Awake()
        {
            _isCombatActive = false;
            Combat_GameState.onEnterState += OnEnterCombatState;
            Combat_GameState.onExitState += OnExitCombatState;
            currentHp = _moduleObject._health;
        }
        

        public virtual void Start()
        { }

        public void Init(ShipController shipController)
        {
            _shipController = shipController;
            _shipController.AddModule(this);
        }

        public virtual void OnDestroy()
        {
            Combat_GameState.onEnterState -= OnEnterCombatState;
            Combat_GameState.onExitState -= OnExitCombatState;
            if (_shipController != null)
            {
                _shipController.RemoveModule(this);
            }
        }

        protected virtual void OnExitCombatState()
        {
            _isCombatActive = false;
        }

        protected virtual void OnEnterCombatState(GameStateController obj)
        {
            _isCombatActive = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.root == transform.root) return;
            

            if (other.transform.TryGetComponent(out Projectile projectile))
            {
                Destroy(other);

                float updatedHP = Mathf.Max(currentHp - projectile._BaseProjectileObject.Damage, 0f);
                float deltaHP = updatedHP - currentHp;
                _shipController.UpdateCurrentHP(deltaHP);
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
            print($"Death {this.GetType().Name}");
        }
    }
}