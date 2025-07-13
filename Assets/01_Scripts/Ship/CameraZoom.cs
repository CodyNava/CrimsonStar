using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _01_Scripts.Ship
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private CameraZoomSettings _cameraZoomSettings;
        [SerializeField] private CameraFollow _cameraFollow;
        public CameraFollow CameraFollow => _cameraFollow;
        public CameraZoomSettings CameraZoomSettings => _cameraZoomSettings;
        private Rigidbody2D _targetRB;

        private float _zoomDistance;
        private float _shipSpeedZoomDistance;
        
        public Action<float> OnChangeZoomDistance;

        public void Awake()
        {
            Assert.IsNotNull(_cameraFollow, "CameraFollow Component needs to be referenced");
        }

        private void OnEnable()
        {
            if(!_cameraFollow.IsUnityNull()) _cameraFollow.OnTargetChanged += OnCameraTargetChanged;
            _zoomDistance = _cameraFollow.CameraDistance;
        }

        private void OnDisable()
        {
            if(!_cameraFollow.IsUnityNull()) _cameraFollow.OnTargetChanged -= OnCameraTargetChanged;
        }

        private void OnCameraTargetChanged(NetBridge target)
        {
            _targetRB = target.GetComponent<Rigidbody2D>();
        } 

        public void Update()
        {
            float scrollValue = -Keybinds.Actions.Camera.CameraZoom.ReadValue<float>();
            OnCameraZoomPerformed(scrollValue);
        }

        private void OnCameraZoomPerformed(float scrollValue)
        {
            float minDist = _cameraZoomSettings.MinDistance;
            float maxDist = _cameraZoomSettings.MaxDistance;

            
            // float currentDistance = -_cameraFollow.CameraDistance;

            float distanceDelta = scrollValue * _cameraZoomSettings.ZoomSpeedFactor;

            if (_cameraZoomSettings.ExpZoomSpeed)
            {
                float currentDistFactor = (_zoomDistance - minDist) / (maxDist - minDist);
                distanceDelta *= Mathf.Max(_cameraZoomSettings.ExpZoomMinSpeed, currentDistFactor);
            }

            _zoomDistance += distanceDelta;

            _zoomDistance = Mathf.Clamp(_zoomDistance, _cameraZoomSettings.MinDistance,
                _cameraZoomSettings.MaxDistance);

            if (_cameraZoomSettings.EnableShipSpeedZoom && !_targetRB.IsUnityNull())
            {
                float velocityMag = _targetRB.linearVelocity.magnitude;
                float maxSpeedPerc = Mathf.Clamp(velocityMag / _cameraFollow.Target.GetMaxMoveSpeed(), 0f, 1f);
                _shipSpeedZoomDistance = (_cameraZoomSettings.MaxDistance * _cameraZoomSettings.ShipSpeedZoomDistFactor) *
                                   maxSpeedPerc;
            }

            _cameraFollow.CameraDistance = -(_zoomDistance + _shipSpeedZoomDistance);
        }

        public void ZoomCamera()
        {
            
            OnChangeZoomDistance?.Invoke(_cameraFollow.CameraDistance);
        }
    }
}