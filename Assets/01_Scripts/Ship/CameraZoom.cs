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

        public Action<float> OnChangeZoomDistance;

        public void Awake()
        {
            Assert.IsNotNull(_cameraFollow, "CameraFollow Component needs to be referenced");
        }

        // public void OnEnable()
        // {
        //     Keybinds.Actions.Camera.CameraZoom.performed -= OnCameraZoomPerformed;
        //     Keybinds.Actions.Camera.CameraZoom.performed += OnCameraZoomPerformed;
        // }
        //
        // public void OnDisable()
        // {
        //     Keybinds.Actions.Camera.CameraZoom.performed -= OnCameraZoomPerformed;
        // }

        public void Update()
        {
            float scrollValue = -Keybinds.Actions.Camera.CameraZoom.ReadValue<float>();
            Debug.Log($"ScrollVal: {scrollValue}");
            if (Mathf.Abs(scrollValue) <= 0.01f) return;
            OnCameraZoomPerformed(scrollValue);
        }

        private void OnCameraZoomPerformed(float scrollValue)
        {
            float minDist = _cameraZoomSettings.MinDistance;
            float maxDist = _cameraZoomSettings.MaxDistance;

            
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