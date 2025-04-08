using System;

namespace _01_Scripts.GameState.States
{
    public abstract class BaseGameState
    {
        public abstract void EnterState(GameStateController gsc);
        public abstract void UpdateState();
        public abstract void ExitState();
    }
    
    public abstract class GameState<T> : BaseGameState where T : GameState<T>
    {
        public static event Action<GameStateController> onEnterState;
        public static event Action onUpdateState;
        public static event Action onExitState;
        

        protected GameStateController _gameStateController;
        
        public override void EnterState(GameStateController gsc)
        {
            _gameStateController = gsc;
            onEnterState?.Invoke(_gameStateController);
            OnStateEnter();
        }

        protected abstract void OnStateEnter();

        public override void UpdateState()
        {
            onUpdateState?.Invoke();
            OnStateUpdate();
        }

        protected abstract void OnStateUpdate();

        public override void ExitState()
        {
            onExitState?.Invoke();
            OnStateExit();
        }

        protected abstract void OnStateExit();
    }
}