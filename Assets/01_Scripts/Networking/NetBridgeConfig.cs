using UnityEngine;

[CreateAssetMenu(fileName = "New Bridge Config", menuName = "Modules/Bridge Config")]
public class NetBridgeConfig : ScriptableObject
{
    [field: SerializeField] public float BaseMovementSpeed { get; private set; }
    [field: SerializeField] public float MaxMovementSpeed { get; private set; }
    [field: SerializeField] public float MaxAngularSpeed { get; private set; }
    [field: SerializeField] public float LinearDampingCoefficient { get; private set; }
    [field: SerializeField] public float AngularDampingCoefficient { get; private set; }
    [field: SerializeField] public float BaseAngularSpeed { get; private set; }
}
