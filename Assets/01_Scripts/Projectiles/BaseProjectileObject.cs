using UnityEngine;

namespace _01_Scripts.Projectiles
{
    [CreateAssetMenu(fileName = "BaseProjectileObject", menuName = "ShipModules/ProjectileModules/BaseProjectileObject")]
    public class BaseProjectileObject : ScriptableObject
    {
        [field:SerializeField] public float ProjectileSpeed { get; private set; }
        [field:SerializeField] public float ProjectileDamage { get; private set; }
        [field:SerializeField] public float ProjectileTimer { get; private set; }
    }
}