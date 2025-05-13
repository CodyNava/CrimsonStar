using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;
using UnityEngine.Assertions;
using Quaternion = UnityEngine.Quaternion;

namespace _01_Scripts.Ship
{
    public class ShipMovementRoll : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float _maxBankAngle;
        [SerializeField] private float _bankSmoothing;
        
        private float _currentBank = 0f;
        
        public void Awake()
        {
            Assert.IsNotNull(_rigidbody2D, "RigidBody2D reference needs to be assigned!");
            Assert.IsNotNull(_targetTransform, "Target Transform reference needs to be assigned");
        }

        public void LateUpdate()
        {
            float steeringAngle = _rigidbody2D.rotation;
            
            float angularVel = _rigidbody2D.angularVelocity;
            float targetBankAngle = Mathf.Clamp(angularVel * _maxBankAngle / 100f, -_maxBankAngle, _maxBankAngle);
            
            _currentBank = Mathf.Lerp(_currentBank, targetBankAngle, Time.deltaTime * _bankSmoothing);
            
            Quaternion steeringRotation = Quaternion.Euler(0f, 0f, steeringAngle);
            Quaternion bankingRotation = Quaternion.Euler(0f, _currentBank, 0f);
            _targetTransform.localRotation = steeringRotation * bankingRotation;
        }
    }
}