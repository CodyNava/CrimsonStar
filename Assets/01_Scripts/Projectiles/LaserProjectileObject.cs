using UnityEngine;

namespace _01_Scripts.Projectiles
{
    [CreateAssetMenu(fileName = "LaserProjectileObject", menuName = "ShipModules/ProjectileModules/LaserProjectileObject")]
    public class LaserProjectileObject : ScriptableObject
    {
        [field:SerializeField] public float MaxLength { get; private set; }
        [field:SerializeField] public float GrowSpeed { get; private set; }
        [field:SerializeField] public float LifetimeAfterFullGrown { get; private set; }
        [field:SerializeField] public float MaxHits { get; private set; }
        [field:SerializeField] public float ProjectileDamage { get; private set; }
    }
}