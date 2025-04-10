using System;
using _01_Scripts.Ship.Modules;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class TurretController : BaseModuleController
    {
        [SerializeField] private GameObject _turretHead;
        [field:SerializeField] public Shooting ShootingController { get; private set; }
        public void SetTargetDestination(Vector2 destination)
        {
            Vector2 direction = (destination - (Vector2)transform.position).normalized; 
            _turretHead.transform.up = direction;
        }
    }
}