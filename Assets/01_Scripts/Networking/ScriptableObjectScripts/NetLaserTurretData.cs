using UnityEngine;

[CreateAssetMenu(fileName = "New Laser Turret Data", menuName = "Turrets/Laser Turret Data")]
public class NetLaserTurretData : ScriptableObject
{
	[field: SerializeField] public float Cooldown { get; private set; }
	[field: SerializeField] public float ChargeTime { get; private set; }
	[field: SerializeField] public NetPredictedProjectileLaser Projectile { get; private set; }
}
