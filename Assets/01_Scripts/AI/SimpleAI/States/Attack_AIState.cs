using UnityEngine;

namespace _01_Scripts.AI.SimpleAI.States
{
    public class Attack_AIState : BaseAIState
    {
        protected override void OnEnterState()
        { }

        protected override void OnUpdateState(float deltaTime)
        {
            if (AIStateMachineCtx.HasTarget)
            {
                AIStateMachineCtx.TriggerMoveTowardsDestination(GetSafeDistanceDestination());
                AIStateMachineCtx.PointTurretsTowardsTarget(AIStateMachineCtx.GetTargetPosition());
                AIStateMachineCtx.TriggerTurretShots();
            }
            // TODO: Move towards safeDistanceDestination via GetSafeDistanceDestination()
            // TODO: Shoot Weapons at TargetPosition
        }

        protected override void OnFixedUpdateState(float deltaTime)
        {
            if (!AIStateMachineCtx.HasTarget)
            {
                AIStateMachineCtx.SetTransitionState(new Patrol_AIState());
                return;
            }
            
            float targetDistanceSqr = AIStateMachineCtx.GetTargetDistanceSqr();
            float attackDistance = AIStateMachineCtx.AIParameters.attackDistance * 1.1f;
            if (targetDistanceSqr > attackDistance * attackDistance)
            {
                AIStateMachineCtx.SetTransitionState(new Chase_AIState());
            }
        }

        protected override void OnExitState()
        { }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetSafeDistanceDestination(), 0.25f);
        }

        private Vector2 GetSafeDistanceDestination()
        {
            float safeDistance = AIStateMachineCtx.AIParameters.safeDistance;
            Vector2 targetPosition = AIStateMachineCtx.GetTargetPosition();
            Vector2 ownPosition = AIStateMachineCtx.transform.position;
            return (ownPosition - targetPosition).normalized * safeDistance + targetPosition;
        }
    }
}