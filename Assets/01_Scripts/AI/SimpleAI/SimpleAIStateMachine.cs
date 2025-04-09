using System;
using _01_Scripts.AI.SimpleAI.States;
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
            return GetTargetTransform().position;
        }

        public float GetTargetDistanceSqr()
        {
            Vector2 towardsPlayer = GetTargetPosition() - (Vector2)transform.position;
            return towardsPlayer.sqrMagnitude;
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