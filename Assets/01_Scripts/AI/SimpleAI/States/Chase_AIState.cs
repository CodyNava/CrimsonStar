using System.Collections.Generic;
using _01_Scripts.Ship.ModuleControllers;
using UnityEngine;

namespace _01_Scripts.AI.SimpleAI.States
{
    public class Chase_AIState : BaseAIState
    {
        private Vector2 _lastTargetPosition;
        
        protected override void OnEnterState()
        { }

        protected override void OnUpdateState(float deltaTime)
        {
            if (AIStateMachineCtx.HasTarget)
            {
                AIStateMachineCtx.TriggerMoveTowardsDestination(_lastTargetPosition);
                AIStateMachineCtx.PointTurretsTowardsTarget(_lastTargetPosition);
            }
        }

        protected override void OnFixedUpdateState(float deltaTime)
        {
            if (!AIStateMachineCtx.HasTarget)
            {
                AIStateMachineCtx.SetTransitionState(new Patrol_AIState());
                return;
            }
            
            float distanceTargetSqr = AIStateMachineCtx.GetTargetDistanceSqr();
            
            float perceptionDistance = AIStateMachineCtx.AIParameters.perceptionDistance;
            if (distanceTargetSqr < perceptionDistance * perceptionDistance)
            {
                _lastTargetPosition = AIStateMachineCtx.GetTargetPosition();
            }

            float attackDistance = AIStateMachineCtx.AIParameters.attackDistance;
            if (distanceTargetSqr < attackDistance * attackDistance)
            {
                AIStateMachineCtx.SetTransitionState(new Attack_AIState());
            }

            float distanceLastTargetPositionSqr = GetDistanceLastTargetPosSqr();
            float reachDistanceMin = AIStateMachineCtx.AIParameters.wanderReachDistanceMin;
            if (distanceLastTargetPositionSqr < reachDistanceMin * reachDistanceMin)
            {
                AIStateMachineCtx.SetTransitionState(new Patrol_AIState());
            }
        }

        protected override void OnExitState()
        { }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_lastTargetPosition, 0.25f);
        }

        private float GetDistanceLastTargetPosSqr()
        {
            Vector2 towardsLastTargetPos = _lastTargetPosition - (Vector2)AIStateMachineCtx.transform.position;
            return towardsLastTargetPos.sqrMagnitude;
        }
        
        
    }
}