using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed = 10f;
    [HideInInspector] public float lifetime = 3f;
    [HideInInspector] public Vector3 direction = Vector3.up;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction.normalized * (speed * Time.deltaTime);
    }
}
