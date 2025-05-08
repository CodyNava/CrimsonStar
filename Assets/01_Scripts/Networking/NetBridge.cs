using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetBridge : NetworkBehaviour
{
    [field: SerializeField] public NetBridgeConfig BridgeConfig { get; private set; }
    [field: SerializeField] public HexTransform HexTransform { get; private set; }
    [field: SerializeField] public Transform VisualRootTransform { get; private set; }

    private readonly SyncVar<NetModuleBaseStats> _baseStats = new();
    public NetModuleBaseStats BaseStats => _baseStats.Value;
    
    public void AddModuleBaseStats(NetModuleBaseStats attachedModuleBaseStats)
    {
        _baseStats.Value = _baseStats.Value.Combine(attachedModuleBaseStats);
    }

    public void DetachModuleBaseStats(NetModuleBaseStats detachedModuleBaseStats)
    {
        _baseStats.Value = _baseStats.Value.Subtract(detachedModuleBaseStats);
    }

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            FindFirstObjectByType<CameraFollow>().SetTargetFollow(VisualRootTransform);
        }
    }
    public override void OnStopClient()
    {
        if (IsOwner)
        {
            FindFirstObjectByType<CameraFollow>()?.SetTargetFollow(null);
        }
    }

    public float ComputeRotationSpeed()
    {
        return BridgeConfig.BaseAngularSpeed + _baseStats.Value.angularThrust / _baseStats.Value.mass;
    }

    public float ComputeMovementSpeed()
    {
        return BridgeConfig.BaseMovementSpeed + _baseStats.Value.thrust / _baseStats.Value.mass;
    }

    public float GetAngularDampingCoefficient()
    {
        return BridgeConfig.AngularDampingCoefficient;
    }

    public float GetLinearDampingCoefficient()
    {
        return BridgeConfig.LinearDampingCoefficient;
    }
}
