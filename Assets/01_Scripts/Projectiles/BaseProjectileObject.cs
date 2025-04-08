using UnityEngine;

namespace _01_Scripts.Projectiles
{
    public class BaseProjectileObject : ScriptableObject
    {
        [SerializeField] private float speed;
        [SerializeField] private float damage;

    }
}