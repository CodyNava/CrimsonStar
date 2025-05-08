using UnityEngine;

[CreateAssetMenu(fileName = "New Turret Data", menuName = "Turrets/Turret Data")]
public class TurretData : ScriptableObject
{
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public PredictedProjectile Projectile { get; private set; }
}
