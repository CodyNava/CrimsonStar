using System;
using _01_Scripts.Projectiles;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed = 10f;
    [HideInInspector] public float lifetime = 3f;
    [HideInInspector] public Vector3 direction = Vector3.up;
    [HideInInspector] public Transform source;

    public BaseProjectileObject _BaseProjectileObject;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction.normalized * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hitted Projectile");
        if (other.transform.root != source)
        {
            Destroy(gameObject);
        }
    }
}
