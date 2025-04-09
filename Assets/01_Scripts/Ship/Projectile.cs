using System;
using _01_Scripts.Projectiles;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed = 10f;
    [HideInInspector] public float lifetime = 3f;
    [HideInInspector] public Vector3 direction = Vector3.up;

    public BaseProjectileObject _BaseProjectileObject;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction.normalized * (speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
