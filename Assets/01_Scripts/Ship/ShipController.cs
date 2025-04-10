using System;
using System.Collections.Generic;
using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.ModuleControllers;
using _01_Scripts.Ship.Modules;
using UnityEngine;

namespace _01_Scripts.Ship
{
    public class ShipController : MonoBehaviour
    {
        public float MaxHp { get; private set; }
        public float MoveSpeedChange { get; private set; }
        
        public Dictionary<string, List<BaseModuleController>> _attachedModuleControllers;
        
        [SerializeField] private bool _isPlayer = false;
        [SerializeField] private BridgeController _bridgeController;

        private float _currentHPPercent = 0f;
        private float _currentHP = 0f;
        
        public void Awake()
        {
            _attachedModuleControllers = new Dictionary<string, List<BaseModuleController>>();
            
            BaseModuleController[] baseModuleControllers = GetComponentsInChildren<BaseModuleController>();
            foreach (BaseModuleController baseModuleController in baseModuleControllers)
            {
                baseModuleController.Init(this);
            }
            
            ShipEditor_GameState.onEnterState += OnEnterShipEditorGameState;
            ShipEditor_GameState.onExitState += OnExitShipEditorGameState;
        }

        public void Start()
        {
            _bridgeController.OnBridgeDestroyed += OnBridgeDestroyed;
        }

        private void OnBridgeDestroyed()
        {
            if (!_isPlayer)
            {
                Destroy(gameObject);
            }
            else
            {
                GameStateController.Instance.ChangeState(new CombatLose_GameState());
            }
        }

        public void OnDestroy()
        {
            ShipEditor_GameState.onEnterState -= OnEnterShipEditorGameState;
            ShipEditor_GameState.onExitState -= OnExitShipEditorGameState;
        }

        private void OnEnterShipEditorGameState(GameStateController obj)
        {

        }

        private void OnExitShipEditorGameState()
        {

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

            result = new List<BaseModuleController>();
            return false;
        }

        public void UpdateCurrentHP(float deltaHP)
        {
            _currentHP += deltaHP;
            if (_currentHP <= 0)
            {
                Debug.Log("Death");
            }
        }
        
        
    }
}