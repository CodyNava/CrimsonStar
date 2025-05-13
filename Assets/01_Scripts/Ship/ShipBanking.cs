using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;
using UnityEngine.Assertions;
using Quaternion = UnityEngine.Quaternion;

namespace _01_Scripts.Ship
{
    public class ShipMovementRoll : MonoBehaviour
    {
        [SerializeField] private NetMovementController _netMovementController;
        [SerializeField] private NetworkObject _netObject;
        private PredictionRigidbody2D _rigidbody2D;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private float _maxBankAngle;
        [SerializeField] private float _bankSmoothing;

        private bool isInitialized = false;
        private float _currentBank = 0f;
        
        public void Awake()
        {
            Assert.IsNotNull(_netMovementController, "NetMovementController Script reference needs to be assigned!");
            Assert.IsNotNull(_netObject, "NetworkObject reference needs to be assigned!");
            Assert.IsNotNull(_visualRoot, "VisualRoot Transform reference needs to be assigned");
        }

        public void Start()
        {
            _rigidbody2D = _netMovementController.PredictionRB;
            isInitialized = true;
        }

        public void LateUpdate()
        {
            if (!_netObject.IsOwner && _netObject.IsClientInitialized)
                return;

            float steeringAngle = _rigidbody2D.Rigidbody2D.rotation;
            
            float angularVel = _rigidbody2D.Rigidbody2D.angularVelocity;
            float targetBankAngle = Mathf.Clamp(angularVel * _maxBankAngle / 100f, -_maxBankAngle, _maxBankAngle);
            
            _currentBank = Mathf.Lerp(_currentBank, targetBankAngle, Time.deltaTime * _bankSmoothing);
            
            Quaternion steeringRotation = Quaternion.Euler(0f, 0f, steeringAngle);
            Quaternion bankingRotation = Quaternion.Euler(0f, _currentBank, 0f);
            _visualRoot.localRotation = steeringRotation * bankingRotation;
        }
    }
}