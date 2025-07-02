using UnityEngine;

namespace _01_Scripts.Projectiles
{
	[CreateAssetMenu(fileName = "RocketProjectileObject", menuName = "ShipModules/ProjectileModules/RocketProjectileObject")]
	public class RocketProjectileObject : ScriptableObject
	{
		[field:SerializeField] public float ProjectileAcceleration { get; private set; }
		[field:SerializeField] public float ProjectileMaxSpeed { get; private set; }
		[field:SerializeField] public float ProjectileDamage { get; private set; }
		[field:SerializeField] public float ProjectileTimer { get; private set; }
		
		[field: SerializeField] public NetPredictedExplosion Explosion { get; private set; }

	}
}