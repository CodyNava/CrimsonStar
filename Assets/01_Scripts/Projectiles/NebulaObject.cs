using UnityEngine;

[CreateAssetMenu(fileName = "Nebula Object", menuName = "EnvironmentModules/Nebula Object")]
public class NebulaObject : ScriptableObject
{
    [field:SerializeField] public float NebulaMinSize { get; private set; }
    [field:SerializeField] public float NebulaMaxSize { get; private set; }
    [field:SerializeField] public float NebulaDamage { get; private set; }
    [field:SerializeField] public float NebulaDamageInterval { get; private set; }
}
