using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ThrusterVFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect thrusterEffect;
    [SerializeField] private InputActionReference thrustInput;
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
        if (thrusterEffect == null)
            return;
        float input = netMovementController.InputThrust;
        bool isThrusting = input > 0.2f;
        float targetStrength = isThrusting ? 1f : 0f;
        _currentStrength = Mathf.MoveTowards(_currentStrength, targetStrength, changeSpeed * Time.deltaTime);
        thrusterEffect.SetFloat("ThrusterStrength", _currentStrength);
       
    }
}
