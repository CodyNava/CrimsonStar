using System;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.AI.SimpleAI.States;
using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.ModuleControllers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts.AI.SimpleAI
{
    public abstract class AIStateMachine : MonoBehaviour
    {
        private BaseAIState _currentAIState;
        private BaseAIState _transitionState;
        [SerializeField] private EnemyMovementController _enemyMovementController;
        [field:SerializeField] public BridgeController BridgeController { get; private set; }
        [SerializeField] private SimpleAIParameters _aiParameters;
        public SimpleAIParameters AIParameters => _aiParameters;

        [SerializeField] private bool _isAIActive = false;
        public bool IsAIActive => _isAIActive;
        private Transform _currentTarget = null; 
        public bool HasTarget => _currentTarget != null;

        public List<BaseModuleController> attachedTurrets = new List<BaseModuleController>();

        public void Awake()
        {
            SetTransitionState(new Patrol_AIState());

            Combat_GameState.onEnterState += OnEnterCombatGameState;
            Combat_GameState.onExitState += OnExitCombatGameState;
        }

        public void OnDestroy()
        {
            Combat_GameState.onEnterState -= OnEnterCombatGameState;
            Combat_GameState.onExitState -= OnExitCombatGameState;
        }

        private void OnExitCombatGameState()
        {
            _isAIActive = false;
            _enemyMovementController.enabled = false;
        }

        private void OnEnterCombatGameState(GameStateController obj)
        {
            _isAIActive = true;
            _enemyMovementController.enabled = true;
        }

        public void Start()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0)
            {
                _currentTarget = players.First().transform;
            }
            
            BridgeController.GetAttachedModuleOfType("TurretController", out attachedTurrets);
            Debug.Log($"Found Modules: {attachedTurrets.Count}");
        }

        private void ChangeState(BaseAIState newAIState)
        {
            _currentAIState?.ExitState();
            _currentAIState = newAIState;
            _currentAIState.EnterState(this);
        }

        public void Update()
        {
            if (!_isAIActive) return;
            
            _currentAIState?.UpdateState(Time.deltaTime);
            
            
            if (_transitionState != null)
            {
                ChangeState(_transitionState);
                _transitionState = null;
            }
        }

        public void FixedUpdate()
        {
            if (!_isAIActive) return;
            
            _currentAIState?.FixedUpdateState(Time.deltaTime);
        }

        public void SetTransitionState(BaseAIState newState)
        {
            Debug.Log($"Transition to State: {newState.GetType().Name}");
            
            // TODO: Resolve problem: What if _transitionState is not NULL?
            //  Overwrite previous _transitionState value?
            
            _transitionState = newState;
        }

        private Transform GetTargetTransform()
        {
            return _currentTarget;
        }

        public Vector2 GetTargetPosition()
        {
            return GetTargetTransform().position;
        }

        public float GetTargetDistanceSqr()
        {
            Vector2 towardsPlayer = GetTargetPosition() - (Vector2)transform.position;
            return towardsPlayer.sqrMagnitude;
        }

        private Vector2 GetMousePosOnPlane()
        {
            Ray worldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane xy = new Plane(new Vector3(0f, 0f, -1f), 0f);
            if (xy.Raycast(worldRay, out float distance))
            {
                return worldRay.GetPoint(distance);
            }
            return Vector2.zero;
        }

        public void TriggerMoveTowardsDestination(Vector2 moveDestination)
        {
            Vector2 direction = moveDestination - (Vector2)transform.position;
            _enemyMovementController.MoveTowards = direction;
        }

        public void PointTurretsTowardsTarget(Vector2 position)
        {
            // if (attachedTurrets.Count <= 0) return;
            foreach (TurretController turret in attachedTurrets)
            {
                turret.SetTargetDestination(position);
            }
        }

        public void TriggerTurretShots()
        {
            // if (attachedTurrets.Count <= 0) return;
            foreach (TurretController turret in attachedTurrets)
            {
                turret.ShootingController._triggerShot = true;
            }
        }

        public void OnDrawGizmos()
        {
            _currentAIState?.DrawGizmos();
            
            Vector2 mousePos = GetMousePosOnPlane();

            switch (_currentAIState?.GetType().Name)
            { 
            default:
            case "Petrol_AIState":
                Gizmos.color = Color.cyan;
                break;
            case "Chase_AIState":
                Gizmos.color = Color.green;
                break;
            case "Attack_AIState":
                Gizmos.color = Color.red;
                break;
            }
            
            Gizmos.DrawWireSphere(mousePos, 1f);
        }

        public void OnDrawGizmosSelected()
        {
            _currentAIState?.DrawGizmosSelected();
            
            Vector3 pos = transform.position;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(pos, AIParameters.wanderDistanceMax);
            Gizmos.DrawWireSphere(pos, AIParameters.wanderDistanceMin);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pos, AIParameters.perceptionDistance);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pos, AIParameters.attackDistance);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pos, AIParameters.safeDistance);
        }
    }
    
    public class SimpleAIStateMachine : AIStateMachine
    {
    }
}