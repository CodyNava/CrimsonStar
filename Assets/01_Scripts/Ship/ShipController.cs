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
                //GameStateController.Instance.ChangeState(new CombatLose_GameState());
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
            AddModuleToList(moduleAdded);
            

            BaseModuleObject moduleObject = moduleAdded.ModuleObject;
            MaxHp += moduleObject._health;
            MoveSpeedChange += moduleObject._moveSpeedChange;
        }
        
        public void RemoveModule(BaseModuleController moduleAdded)
        {
            RemoveModuleToList(moduleAdded);
            
            BaseModuleObject moduleObject = moduleAdded.ModuleObject;
            MaxHp -= moduleObject._health;
            MoveSpeedChange -= moduleObject._moveSpeedChange;
        }
        
        // TODO: Segregate BaseModule as Interface
        
        private void AddModuleToList(BaseModuleController moduleAdded) 
        {
            string moduleName = moduleAdded.GetType().Name;
            if (!_attachedModuleControllers.ContainsKey(moduleName))
            {
                _attachedModuleControllers.Add(moduleName, new List<BaseModuleController>());
            }
            _attachedModuleControllers[moduleName].Add(moduleAdded);
        }
        
        private bool RemoveModuleToList(BaseModuleController moduleAdded)
        {
            string moduleName = moduleAdded.GetType().Name;
            if (!_attachedModuleControllers.ContainsKey(moduleName)) return false;

            _attachedModuleControllers[moduleName].Remove(moduleAdded);
            return true;
        }

        // TODO: Prevent allocation of new List onto out parameter
        public bool GetAttachedModulesOfType<T>(out List<T> result) where T : BaseModuleController
        {
            string className = typeof(T).Name;
            if (_attachedModuleControllers.ContainsKey(className))
            {
                result = _attachedModuleControllers[className].ConvertAll(module => (T)module);
                return true;
            }

            result = new List<T>();
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