using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.Modules;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class BridgeController : BaseModuleController
    {
        public BridgeModuleObject BridgeObject => (BridgeModuleObject)ModuleObject;
        public float MaxHp { get; private set; }
        public float MoveSpeedChange { get; private set; }

        public override void Awake()
        {
            base.Awake();
            BridgeController = this;
            AddModule(BridgeController.ModuleObject);
        }

        public void AddModule(BaseModuleObject modulAdded)
        {
            MaxHp += modulAdded._health;
            MoveSpeedChange += modulAdded._moveSpeedChange;
        }
        
        public void RemoveModule(BaseModuleObject modulAdded)
        {
            MaxHp -= modulAdded._health;
            MoveSpeedChange -= modulAdded._moveSpeedChange;
        }
        public void ModifyCurrentHp(float delta)
        {
            MaxHp += delta;
        }

        protected override void OnModuleDestroyed()
        {
            GameStateController.Instance.ChangeState(new CombatLose_GameState());
        }
    }
}