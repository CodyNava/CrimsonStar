using UnityEngine;

namespace _01_Scripts.GameState.States
{
    public class ShipEditor_GameState : GameState<ShipEditor_GameState>
    {
        protected override void OnStateEnter()
        {
            Debug.Log("Enter ShipEditor_State");
        }

        protected override void OnStateUpdate()
        {
        }

        protected override void OnStateExit()
        {
            Debug.Log("Exit ShipEditor_State");
        }
    }
}