using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class ThrusterController : BaseModuleController
    {
        public Player player;
        public GameObject thrusterVFX;
        public void Start()
        {
            player = GetComponentInParent<Player>();
        }
        public void Update()
        {
            EnableThrusterVFX();
        }
        public void EnableThrusterVFX()
        {
            if (player.isAccelerating || player.isRotating)
            {
                thrusterVFX.SetActive(true);
            }
            else if (!player.isAccelerating && !player.isRotating)
            {
                thrusterVFX.SetActive(false);
            }
        }
    }
}