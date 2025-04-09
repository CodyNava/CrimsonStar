using System;
using _01_Scripts.AI.SimpleAI.States;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts.AI.SimpleAI
{
    public abstract class AIStateMachine : MonoBehaviour
    {
        private BaseAIState _currentAIState;
        private BaseAIState _transitionState;

        [SerializeField] private SimpleAIParameters _aiParameters;
        public SimpleAIParameters AIParameters => _aiParameters;

        public void Awake()
        {
            SetTransitionState(new Patrol_AIState());
        }

        public void ChangeState(BaseAIState newAIState)
        {
            _currentAIState?.ExitState();
            _currentAIState = newAIState;
            _currentAIState.EnterState(this);
        }

        public void Update()
        {
            _currentAIState?.UpdateState(Time.deltaTime);
            
            
            if (_transitionState != null)
            {
                ChangeState(_transitionState);
                _transitionState = null;
            }
        }

        public void FixedUpdate()
        {
            _currentAIState?.FixedUpdateState(Time.deltaTime);
        }

        public void SetTransitionState(BaseAIState newState)
        {
            Debug.Log($"Transition to State: {newState.GetType().Name}");
            
            // TODO: Resolve problem: What if _transitionState is not NULL?
            //  Overwrite previous _transitionState value?
            _transitionState = newState;
        }

        protected abstract Transform GetTargetTransform();

        public Vector2 GetTargetPosition()
        {
            return GetMousePosOnPlane();
            // return GetTargetTransform().position;
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
        protected override Transform GetTargetTransform()
        {
            // TODO: Determine an algorithm to choose a Target
            //  Probably the Player GameObject as Transform
            throw new NotImplementedException();
        }
    }
}