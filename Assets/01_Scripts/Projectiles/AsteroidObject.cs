using UnityEngine;

[CreateAssetMenu(fileName = "Asteroid Object", menuName = "EnvironmentModules/Asteroid Object")]
public class AsteroidObject : ScriptableObject
{
    [field:SerializeField] public int AsteroidMinSize { get; private set; }
    [field:SerializeField] public int AsteroidMaxSize { get; private set; }
    [field:SerializeField] public float AsteroidDamage { get; private set; }
}
