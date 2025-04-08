using UnityEngine;

namespace _01_Scripts.GameState.States
{
    public class Combat_GameState : GameState<Combat_GameState>
    {
        protected override void OnStateEnter()
        {
            Debug.Log("Enter Combat_State");
        }

        protected override void OnStateUpdate()
        {
        }

        protected override void OnStateExit()
        {
            Debug.Log("Exit Combat_State");
        }
    }
}