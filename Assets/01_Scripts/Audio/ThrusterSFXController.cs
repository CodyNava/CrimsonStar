using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrusterSFXController : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference thrusterSoundEvent;
    [SerializeField] private InputActionReference thrustInput;
    [SerializeField] private float changeSpeed = 2f;
    private float _currentStrength;

    FMOD.Studio.EventInstance _thrusterSound;

    void Start()
    {
        _thrusterSound = FMODUnity.RuntimeManager.CreateInstance(thrusterSoundEvent);
        _thrusterSound.start();
    }

    private void Update()
    {
        SetThrusterSFXStrenght();
    }

    private void SetThrusterSFXStrenght()
    {
        Vector2 input = thrustInput.action.ReadValue<Vector2>();
        bool isThrusting = input.y > 0.2f;
        float targetStrenght = isThrusting ? 1f : 0f;
        _currentStrength = Mathf.MoveTowards(_currentStrength, targetStrenght, changeSpeed * Time.deltaTime);
        _thrusterSound.setParameterByName("Thrust", _currentStrength);
    }

    private void OnDestroy()
    {
        {
            Debug.Log("Thruster OnDestroy");
            _thrusterSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _thrusterSound.release();
        }
    }
}