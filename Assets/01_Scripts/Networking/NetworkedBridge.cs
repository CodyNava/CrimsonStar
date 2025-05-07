using FishNet.Component.Transforming;
using FishNet.Object;
using UnityEngine;

public class NetworkedBridge : NetworkBehaviour
{
    [field: SerializeField] public HexTransform HexTransform { get; private set; }
    [field: SerializeField] public NetworkTransform NetworkTransform { get; private set; }

    private NetModuleBaseStats _baseStats;

    public void AddModuleBaseStats(NetModuleBaseStats attachedModuleBaseStats)
    {
        _baseStats = _baseStats.Combine(attachedModuleBaseStats);
    }

    public void DetachModuleBaseStats(NetModuleBaseStats detachedModuleBaseStats)
    {
        _baseStats = _baseStats.Subtract(detachedModuleBaseStats);
    }

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            FindFirstObjectByType<CameraFollow>().SetTargetFollow(transform);
        }
    }
    public override void OnStopClient()
    {
        if (IsOwner)
        {
            FindFirstObjectByType<CameraFollow>().SetTargetFollow(null);
        }
    }
}
