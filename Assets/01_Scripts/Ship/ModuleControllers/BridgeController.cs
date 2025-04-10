using System.Collections.Generic;
using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.Modules;
using Unity.VisualScripting;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class BridgeController : BaseModuleController
    {
        public BridgeModuleObject BridgeObject => (BridgeModuleObject)ModuleObject;
        public float MaxHp { get; private set; }
        public float MoveSpeedChange { get; private set; }

        public Dictionary<string, List<BaseModuleController>> _attachedModuleControllers;

        public override void Awake()
        {
            base.Awake();
            _attachedModuleControllers = new Dictionary<string, List<BaseModuleController>>();

            BridgeController = this;
            AddModule(BridgeController.ModuleObject);
        }

        public override void Start()
        {
            base.Start();
            BaseModuleController[] baseModuleControllers = GetComponentsInChildren<BaseModuleController>();
            foreach (BaseModuleController baseModuleController in baseModuleControllers)
            {
                AddModule(baseModuleController);
            }
            
            Debug.Log($"{_attachedModuleControllers.Count}");
            
            List<BaseModuleController> list;
            if (GetAttachedModuleOfType("TurretController", out list))
            {
                foreach (BaseModuleController moduleController in list)
                {
                    Debug.Log(moduleController);
                }
            }
            
        }

        public void AddModule(BaseModuleController moduleAdded)
        {
            AddModuleList(moduleAdded);
            

            BaseModuleObject moduleObject = moduleAdded.ModuleObject;
            MaxHp += moduleObject._health;
            MoveSpeedChange += moduleObject._moveSpeedChange;
        }
        
        public void RemoveModule(BaseModuleController moduleAdded)
        {
            RemoveModuleList(moduleAdded);
            
            BaseModuleObject moduleObject = moduleAdded.ModuleObject;
            MaxHp -= moduleObject._health;
            MoveSpeedChange -= moduleObject._moveSpeedChange;
        }
        
        private void AddModuleList(BaseModuleController moduleAdded) 
        {
            string moduleName = moduleAdded.GetType().Name;
            if (!_attachedModuleControllers.ContainsKey(moduleName))
            {
                _attachedModuleControllers.Add(moduleName, new List<BaseModuleController>());
            }
            _attachedModuleControllers[moduleName].Add(moduleAdded);
        }

        private void RemoveModuleList(BaseModuleController moduleAdded)
        {
            string moduleName = moduleAdded.GetType().Name;
            if (!_attachedModuleControllers.ContainsKey(moduleName)) return;

            _attachedModuleControllers[moduleName].Remove(moduleAdded);
        }

        public bool GetAttachedModuleOfType(string moduleName, out List<BaseModuleController> result)
        {
            if (_attachedModuleControllers.ContainsKey(moduleName))
            {
                result = _attachedModuleControllers[moduleName];
                return true;
            }

            result = null;
            return false;
        }
        public void ModifyCurrentHp(float delta)
        {
            MaxHp += delta;
        }

        protected override void OnModuleDestroyed()
        {
            GameStateController.Instance.ChangeState(new CombatLose_GameState());
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