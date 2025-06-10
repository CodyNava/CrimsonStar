using System;
using System.Collections.Generic;
using _01_Scripts.Ship.Modules;
using UnityEngine;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class BridgeController : BaseModuleController
    {
        public BridgeModuleObject BridgeObject => (BridgeModuleObject)ModuleObject;

        public float Mass => 350f;

        public event Action OnBridgeDestroyed;
        
        protected override void OnModuleDestroyed()
        {
            OnBridgeDestroyed?.Invoke();
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            ContactPoint2D contact = collision.GetContact(0);

            Collider2D localCollider = contact.otherCollider;
            Collider2D otherCollider = contact.collider;
            
            // Debug.Log($"Bridge: local: {localCollider}; remote: {otherCollider}");
        }
    }
}