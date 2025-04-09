using UnityEngine;
using UnityEngine.Serialization;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "BridgeObject", menuName = "ShipModules/BridgeObject")]
    public class BridgeModuleObject : BaseModuleObject
    {
        public float baseMoveSpeed = 0.04f;
        public float maxSpeed = 8f;
        public float maxAngularVelocity = 180f;
        public float movementDamping = 5f;
        public float rotationDamping = 5f;
        public float rotationSpeed = 1f;
    }
}