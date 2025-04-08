using System;
using _01_Scripts.GameState.States;
using UnityEngine;

namespace _01_Scripts.GameState
{
    public class GameStateController : MonoBehaviour
    {
        public static GameStateController Instance { get; private set; }
        
        private BaseGameState _currentGameState;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            
            ChangeState(new ShipEditor_GameState());
        }


        public void ChangeState(BaseGameState newGameState)
        {
            _currentGameState?.ExitState();
            _currentGameState = newGameState;
            _currentGameState.EnterState(this);
        }

        private void Update()
        {
            _currentGameState?.UpdateState();
        }
    }
}