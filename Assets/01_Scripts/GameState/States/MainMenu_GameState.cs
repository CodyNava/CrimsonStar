using UnityEngine;

namespace _01_Scripts.GameState.States
{
    public class MainMenu_GameState : GameState<MainMenu_GameState>
    {
        protected override void OnStateEnter()
        {
            Debug.Log("Entered MainMenu GameState");
        }

        protected override void OnStateUpdate()
        {
            Debug.Log("Updated MainMenu GameState");
        }

        protected override void OnStateExit()
        {
            Debug.Log("Exited MainMenu GameState");
        }
    }
}