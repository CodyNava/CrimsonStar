using UnityEngine;

[CreateAssetMenu(fileName = "New Rocket Turret Data", menuName = "Turrets/Rocket Turret Data")]
public class NetRocketTurretData : ScriptableObject
{
	[field: SerializeField] public float Cooldown { get; private set; }
	[field: SerializeField] public NetPredictedProjectileRocket Projectile { get; private set; }
	[field: SerializeField] public NetPredictedExplosion Explosion { get; private set; }
}