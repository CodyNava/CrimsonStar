using FishNet.Object;
using UnityEngine;

public class NetGameplayModule : NetworkBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    
    private NetworkedBridge _bridge;

    public void LinkToBridge(NetworkedBridge bridge)
    {
        _bridge = bridge;
        _bridge.AddModuleBaseStats(ModuleID.GetModuleData().BaseStats);
    }

    public override void OnStartClient()
    {
        _bridge = GetComponentInParent<NetworkedBridge>();
        VisualTransform.SetParent(_bridge.VisualRootTransform);
    }
}
