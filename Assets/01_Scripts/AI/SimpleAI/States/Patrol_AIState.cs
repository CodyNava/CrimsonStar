using UnityEngine;

namespace _01_Scripts.AI.SimpleAI.States
{
    public class Patrol_AIState : BaseAIState
    {
        private bool _hasActiveWanderDestination = false;
        private Vector2 _wanderDestination;
        private float _wanderReachDistance;
        
        protected override void OnEnterState()
        { }

        protected override void OnUpdateState(float deltaTime)
        {
            // TODO: Move towards wanderPosition
        }

        protected override void OnFixedUpdateState(float deltaTime)
        {
            float distanceTowardsWanderPoint =
                GetDistanceToWanderPosSqr(_wanderDestination, AIStateMachineCtx.transform.position);
            
            if (!_hasActiveWanderDestination || distanceTowardsWanderPoint < _wanderReachDistance * _wanderReachDistance)
            {
                SetNewWanderTarget();
            }
            
            float distancePlayerSqr = AIStateMachineCtx.GetTargetDistanceSqr();
            float perceptionDistance = AIStateMachineCtx.AIParameters.perceptionDistance;
            if (distancePlayerSqr < perceptionDistance * perceptionDistance)
            {
                AIStateMachineCtx.SetTransitionState(new Chase_AIState());
            }
        }

        protected override void OnExitState()
        { }

        private void SetNewWanderTarget()
        {
            float wanderDistance = ChooseWanderDistance();
            Vector2 ownPosition = AIStateMachineCtx.transform.position;
            _wanderDestination = ChooseWanderPosition(wanderDistance, ownPosition);
            
            _wanderReachDistance = ChooseWanderReachDistance();

            _hasActiveWanderDestination = true;
        }

        private float ChooseWanderReachDistance()
        {
            float min = AIStateMachineCtx.AIParameters.wanderReachDistanceMin;
            float max = AIStateMachineCtx.AIParameters.wanderReachDistanceMax;
            return Random.Range(min, max);
        }

        private float ChooseWanderDistance()
        {
            float min = AIStateMachineCtx.AIParameters.wanderDistanceMin;
            float max = AIStateMachineCtx.AIParameters.wanderDistanceMax;
            return Random.Range(min, max);
        }

        private Vector2 ChooseWanderPosition(float wanderDistance, Vector2 position)
        {
            return Random.insideUnitCircle.normalized * wanderDistance + position;
        }

        private float GetDistanceToWanderPosSqr(Vector2 wanderPosition, Vector2 position)
        {
            Vector2 dir = wanderPosition - position;
            return dir.sqrMagnitude;
        }
    }
}