using System;
using System.Collections.Generic;
using _01_Scripts.Ship.Modules;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class BridgeController : BaseModuleController
    {
        [SerializeField] private ShipController _shipController;
        public BridgeModuleObject BridgeObject => (BridgeModuleObject)ModuleObject;

        public event Action OnBridgeDestroyed;
        
        protected override void OnModuleDestroyed()
        {
            OnBridgeDestroyed?.Invoke();
        }
    }
}