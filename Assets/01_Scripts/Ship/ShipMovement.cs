using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float moveForce;
    public float rotation;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }
    public void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * moveForce, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * moveForce, ForceMode.Acceleration);
        }
    }
    public void Rotate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(Vector3.back * -rotation, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddTorque(Vector3.back * rotation, ForceMode.Acceleration);
        }
    }
}
