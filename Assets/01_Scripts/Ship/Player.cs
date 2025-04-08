using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    
    public float moveSpeed = 0.1f;
    private Vector2 input;
    private Vector3 velocity;
    public float weight = 100f;
    public float maxSpeed = 20;
    public float rotationSpeed = 180f;

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
        Vector3 movement = (transform.up * input.y).normalized;

        velocity += moveSpeed * movement;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (input.magnitude <= 0.0001f)
        {
            velocity *= 1f - (weight / 1000f);
        }

        if (velocity.magnitude <= 0.0001f)
        {
            velocity = Vector3.zero;
        }

        transform.Translate(velocity * Time.deltaTime, Space.World);
        
        
        
        transform.Rotate(0f, 0f, -input.x * rotationSpeed * Time.deltaTime);
        
        
    }
}
