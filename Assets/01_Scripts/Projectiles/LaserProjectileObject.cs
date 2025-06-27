using UnityEngine;

namespace _01_Scripts.Projectiles
{
    [CreateAssetMenu(fileName = "ProjectileObject", menuName = "ShipModules/ProjectileObject")]
    public class LaserProjectileObject : ScriptableObject
    {
        [field:SerializeField] public float Speed { get; private set; }
        [field:SerializeField] public float Damage { get; private set; }

    }
}