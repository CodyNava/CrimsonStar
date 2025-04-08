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
        Vector3 movement = new Vector3(input.x, input.y, 0).normalized;
        velocity += moveSpeed * movement;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        
        if (input.magnitude <= 0.0001)
        {
            velocity *= 1f - (weight / 1000f);
        }
        
        if (velocity.magnitude <= 0.0001)
        {
            velocity = Vector3.zero;
        }
        transform.Translate(velocity * Time.deltaTime, Space.World);
        
        if (movement != Vector3.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        }
        
        
    }
}
