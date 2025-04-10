using UnityEngine;

namespace _01_Scripts.Ship
{
    public class PlayerShooting : Shooting
    {
        private Turet playerInput;
        
        protected override void Awake()
        {
            base.Awake();
            playerInput = new Turet();
            playerInput.Enable();
        }

        protected override void Update()
        {
            base.Update();
            _triggerShot = playerInput.Player.Attack.IsPressed();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            playerInput.Disable();
        }
    }
}