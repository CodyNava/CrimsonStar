using System;
using _01_Scripts.Ship.Modules;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class TurretController : BaseModuleController
    {
        [SerializeField] private GameObject _turretHead;
        
        
        
        public Vector2 TargetDestination
        {
            get => TargetDestination;
            set
            {
                TargetDestination = value;
                Vector2 direction = (TargetDestination - (Vector2)transform.position).normalized; 
                _turretHead.transform.up = direction;
            }
        }

        public void triggerShoot()
        {
        }
    }
}