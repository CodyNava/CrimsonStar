using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetGameplayModule : NetworkBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    
    private NetBridge _bridge;
    private readonly SyncVar<float> _health = new();
    private readonly SyncVar<NetPlayerID> _playerID = new();
    
    public float Health => _health.Value;
    public NetPlayerID NetPlayerID => _playerID.Value;
    
    public void ServerInit(NetBridge bridge, NetPlayerID netPlayerID)
    {
        _bridge = bridge;
        _bridge.AddModuleBaseStats(ModuleID.GetModuleData().BaseStats);
        _health.Value = ModuleID.GetModuleData().BaseStats.health;
        _playerID.Value = netPlayerID;
    }

    public override void OnStartClient()
    {
        _bridge = ModuleID == NetModuleID.Bridge ? GetComponent<NetBridge>() : GetComponentInParent<NetBridge>();
        VisualTransform.SetParent(_bridge.VisualRootTransform);
    }

    private void DetachModule()
    {
        _bridge.DetachModuleBaseStats(ModuleID.GetModuleData().BaseStats);
        DetachModuleObserver();
        Destroy(VisualTransform.gameObject);
        Destroy(gameObject);
    }

    [ObserversRpc]
    public void DetachModuleObserver()
    {
        Destroy(VisualTransform.gameObject);
        Destroy(gameObject);
    }

    public void InflictDamage(float damage)
    {
        _health.Value -= damage;
        if (_health.Value <= 0)
        {
            DetachModule();
        }
    }
}
