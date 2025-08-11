using UnityEngine;

[CreateAssetMenu(fileName = "Asteroid Object", menuName = "EnvironmentModules/Asteroid Object")]
public class AsteroidObject : ScriptableObject
{
    [field:SerializeField] public float AsteroidMinSize { get; private set; }
    [field:SerializeField] public float AsteroidMaxSize { get; private set; }
    [field:SerializeField] public float AsteroidDamage { get; private set; }
    [field: SerializeField] public Asteroid AsteroidPrefab { get; private set; }
    [field: SerializeField] public float AsteroidSpawnInterval { get; private set; }
    [field: SerializeField] public float AsteroidSpawnerWidth { get; private set; }
    [field: SerializeField] public float AsteroidSpawnerHeight { get; private set; }
    [field: SerializeField] public float AsteroidMinSpeed { get; private set; }
    [field: SerializeField] public float AsteroidMaxSpeed { get; private set; }
    [field: SerializeField] public int AsteroidPenetrationHits { get; private set; }
}
