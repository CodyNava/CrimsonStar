using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Object", menuName = "ShipModules/ProjectileModules/Explosion Object")]
public class ExplosionObject : ScriptableObject
{
	[field:SerializeField] public float ExplosionTimer { get; private set; }
	[field:SerializeField] public float ExplosionMinSize { get; private set; }
	[field:SerializeField] public float ExplosionMaxSize { get; private set; }
}