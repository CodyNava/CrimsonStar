using System;
using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship;
using _01_Scripts.Ship.ModuleControllers;
using _01_Scripts.Ship.Modules;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private ShipController _shipController;
    [SerializeField] private BridgeController _bridgeController;
    private BridgeModuleObject _bridgeModuleObject;

    [SerializeField] private float controllerDeadZone = 0.1f;

    [SerializeField] AudioClip engineSound;
    [SerializeField] AudioSource audioSource;

    private bool _isCombatActive = false;
    
    private Vector2 input;
    private Vector3 velocity;
    private float angularVelocity;
    public bool isAccelerating;
    public bool isRotating;

    public void Awake()
    {
        _bridgeModuleObject = _bridgeController.BridgeObject;
        Combat_GameState.onEnterState += OnEnterCombatGameState;
        Combat_GameState.onExitState += OnExitCombatGameState;
    }

    public void OnDestroy()
    {
        Combat_GameState.onEnterState -= OnEnterCombatGameState;
        Combat_GameState.onExitState -= OnExitCombatGameState;
    }

    private void OnEnterCombatGameState(GameStateController obj)
    {
        _isCombatActive = true;
    }

    private void OnExitCombatGameState()
    {
        _isCombatActive = false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    private void Update()
    {   
        if(_isCombatActive)
        {
            movePlayer();
        }
    }

    public void movePlayer()
    {
        if (input.magnitude < controllerDeadZone)
            input = Vector2.zero;

        if (Mathf.Abs(input.x) > 0.01f)
        {
            angularVelocity += -input.x * _bridgeModuleObject.rotationSpeed;
            isAccelerating = true;
            if (!audioSource.isPlaying && (isAccelerating || isRotating))
            {
                audioSource.PlayOneShot(engineSound);
            }
        }
        else
        {
            angularVelocity *= 1f - (_bridgeModuleObject.rotationDamping / 1000f);
            isAccelerating = false;
            if (audioSource.isPlaying && !isAccelerating && !isRotating)
            {
                audioSource.Stop();
            }
        }

        angularVelocity = Mathf.Clamp(angularVelocity, -_bridgeModuleObject.maxAngularVelocity,
            _bridgeModuleObject.maxAngularVelocity);

        transform.Rotate(0f, 0f, angularVelocity * Time.deltaTime);


        if (Mathf.Abs(input.y) > 0.01f)
        {
            Vector3 movement = transform.up * input.y;
            float totalMoveSpeed = Mathf.Max(_bridgeModuleObject.baseMoveSpeed + _shipController.MoveSpeedChange, 0f);
            velocity += totalMoveSpeed * movement.normalized;
            isRotating = true;
            if (!audioSource.isPlaying && (isAccelerating || isRotating))
            {
                audioSource.PlayOneShot(engineSound);
            }
        }
        else
        {
            velocity *= 1f - (_bridgeModuleObject.movementDamping / 1000f);
            isRotating = false;
            if (audioSource.isPlaying && !isAccelerating && !isRotating)
            {
                audioSource.Stop();
            }
        }

        velocity = Vector3.ClampMagnitude(velocity, _bridgeModuleObject.maxSpeed);

        if (velocity.magnitude <= 0.0001f)
        {
            velocity = Vector3.zero;
        }

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
}