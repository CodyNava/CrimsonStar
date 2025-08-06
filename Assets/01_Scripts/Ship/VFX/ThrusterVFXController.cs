using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ThrusterVFXController : MonoBehaviour
{
    [SerializeField] private List<VisualEffect> thrusterEffect;
    [SerializeField] private float changeSpeed = 2f;
    [SerializeField] private NetMovementController netMovementController;
    private float _currentStrength = 0f;

    void Update()
    {
        SetThrusterStrenghtIfInput();
    }

    private void Start()
    {
        netMovementController = GetComponentInParent<NetMovementController>();
    }

    public void SetThrusterStrenghtIfInput()
    {
        if (thrusterEffect == null || netMovementController == null)
            return;

        bool isGamepad = Gamepad.current != null && Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.01f;

        float thrustCheck = 0f;
        if (isGamepad)
        {
            Vector2 gamepadInput = Gamepad.current.leftStick.ReadValue();
            thrustCheck = gamepadInput.magnitude;
        }
        else
        {
            thrustCheck = netMovementController.InputThrust;
        }

        bool isThrusting = thrustCheck > 0.2f;
        float targetStrength = isThrusting ? 1f : 0f;

        _currentStrength = Mathf.MoveTowards(_currentStrength, targetStrength, changeSpeed * Time.deltaTime);
        foreach (var vfx in thrusterEffect)
        {
            vfx.SetFloat("ThrusterStrength", _currentStrength);
        }
    }
}