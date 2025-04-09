namespace _01_Scripts.AI.SimpleAI.States
{
    public abstract class BaseAIState
    {
        private AIStateMachine _aiStateMachine;
        protected AIStateMachine AIStateMachineCtx => _aiStateMachine;

        public void EnterState(AIStateMachine aiStateMachine)
        {
            _aiStateMachine = aiStateMachine;
            OnEnterState();
        }
        protected abstract void OnEnterState();

        public void UpdateState(float deltaTime)
        {
            OnUpdateState(deltaTime);
        }
        protected abstract void OnUpdateState(float deltaTime);

        public void FixedUpdateState(float deltaTime)
        {
            OnFixedUpdateState(deltaTime);
        }
        protected abstract void OnFixedUpdateState(float deltaTime);

        public void ExitState()
        {
            OnExitState();
        }
        protected abstract void OnExitState();
    }
}