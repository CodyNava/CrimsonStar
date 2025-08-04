using UnityEngine;

[CreateAssetMenu(fileName = "Nebula Object", menuName = "EnvironmentModules/Nebula Object")]
public class NebulaObject : ScriptableObject
{
    [field:SerializeField] public int NebulaMinSize { get; private set; }
    [field:SerializeField] public int NebulaMaxSize { get; private set; }
    [field:SerializeField] public float NebulaDamage { get; private set; }
    [field:SerializeField] public float NebulaDamageInterval { get; private set; }
}
