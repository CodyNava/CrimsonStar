using System;
using _01_Scripts.Ship.ModuleControllers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private BridgeController _bridgeController;
    
    [SerializeField] private float moveSpeed = 0.04f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float maxAngularVelocity = 180f;
    [SerializeField] private float movementDamping = 5f;
    [SerializeField] private float rotationDamping = 5f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float controllerDeadZone = 0.1f;
    
    private Vector2 input;
    private Vector3 velocity;
    private float angularVelocity;

    public void Awake()
    {
        _bridgeController = GetComponent<BridgeController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        if (_bridgeController != null)
        {
            if(_bridgeController.IsCombatActive)
                movePlayer();
        }
        else
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
            angularVelocity += -input.x * rotationSpeed;
        }
        else
        {
            angularVelocity *= 1f - (rotationDamping / 1000f);
        }
        
        angularVelocity = Mathf.Clamp(angularVelocity, -maxAngularVelocity, maxAngularVelocity);
        
        transform.Rotate(0f, 0f, angularVelocity * Time.deltaTime);
        
        
        if (Mathf.Abs(input.y) > 0.01f)
        {
            Vector3 movement = transform.up * input.y;
            velocity += moveSpeed * movement.normalized;
        }
        else
        {
            velocity *= 1f - (movementDamping / 1000f);
        }

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (velocity.magnitude <= 0.0001f)
        {
            velocity = Vector3.zero;
        }

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
}
