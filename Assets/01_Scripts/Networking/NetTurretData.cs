using UnityEngine;

[CreateAssetMenu(fileName = "New Turret Data", menuName = "Turrets/Turret Data")]
public class NetTurretData : ScriptableObject
{
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public NetPredictedProjectile Projectile { get; private set; }
}
