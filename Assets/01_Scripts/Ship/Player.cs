using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    
    public float moveSpeed = 0.04f;
    public float maxSpeed = 8f;
    public float maxAngularVelocity = 180f;
    public float movementDamping = 5f;
    public float rotationDamping = 5f;
    public float rotationSpeed = 1f;
    public float controlerDeadZone = 0.1f;
    
    private Vector2 input;
    private Vector3 velocity;
    private float angularVelocity;
    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        movePlayer();
    }

    public void movePlayer()
    {
        if (input.magnitude < controlerDeadZone)
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
