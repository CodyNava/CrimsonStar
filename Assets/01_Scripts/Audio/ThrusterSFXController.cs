using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrusterSFXController : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference thrusterSoundEvent;
    [SerializeField] private NetMovementController netMovementController;
    [SerializeField] private float changeSpeed = 2f;
    [SerializeField] private float maxRange;
    [SerializeField] private float minRange;
    private float _currentStrength;

    EventInstance _thrusterSound;

    void Start()
    {
        _thrusterSound.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minRange);
        _thrusterSound.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxRange);

        _thrusterSound = FMODUnity.RuntimeManager.CreateInstance(thrusterSoundEvent);
        _thrusterSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        _thrusterSound.start();
    }

    private void Update()
    {
        _thrusterSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        SetThrusterSFXStrenght();
    }

    private void SetThrusterSFXStrenght()
    {
        float input = netMovementController.InputThrust;
        bool isThrusting = input > 0.2f;
        float targetStrenght = isThrusting ? 1f : 0f;
        _currentStrength = Mathf.MoveTowards(_currentStrength, targetStrenght, changeSpeed * Time.deltaTime);
        _thrusterSound.setParameterByName("Thrust", _currentStrength);
    }

    private void OnDestroy()
    {
        {
            _thrusterSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _thrusterSound.release();
        }
    }
}