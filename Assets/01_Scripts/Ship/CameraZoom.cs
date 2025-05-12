using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace _01_Scripts.Ship
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private CameraZoomSettings _cameraZoomSettings;
        [SerializeField] private CameraFollow _cameraFollow;
        [SerializeField] private InputActionAsset _inputActionAsset;

        private InputAction _cameraZoomAction;

        public Action<float> OnChangeZoomDistance;

        public void Awake()
        {
            Assert.IsNotNull(_cameraFollow, "CameraFollow Component needs to be referenced");
            Assert.IsNotNull(_inputActionAsset, "InputActionAssets needs to be set");

            InputActionMap cameraMap = _inputActionAsset.FindActionMap("Camera", true);
            _cameraZoomAction = cameraMap.FindAction("CameraZoom", true);
        }

        public void OnEnable()
        {
            _cameraZoomAction.performed += OnCameraZoomPerformed;
        }

        public void OnDisable()
        {
            _cameraZoomAction.performed -= OnCameraZoomPerformed;
        }

        private void OnCameraZoomPerformed(InputAction.CallbackContext ctx)
        {
            float minDist = _cameraZoomSettings.MinDistance;
            float maxDist = _cameraZoomSettings.MaxDistance;
            
            float scrollValue = -ctx.ReadValue<float>();
            float currentDistance = -_cameraFollow.CameraDistance;

            float distanceDelta = scrollValue * _cameraZoomSettings.ZoomSpeedFactor;

            if (_cameraZoomSettings.ExpZoomSpeed)
            {
                float currentDistFactor = (currentDistance - minDist) / (maxDist - minDist);
                distanceDelta *= Mathf.Max(_cameraZoomSettings.ExpZoomMinSpeed, currentDistFactor);
            }

            currentDistance += distanceDelta;

            currentDistance = Mathf.Min(_cameraZoomSettings.MaxDistance,
                Mathf.Max(_cameraZoomSettings.MinDistance, currentDistance));

            _cameraFollow.CameraDistance = -currentDistance;
        }

        public void ZoomCamera()
        {
            
            OnChangeZoomDistance?.Invoke(_cameraFollow.CameraDistance);
        }
    }
}