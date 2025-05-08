using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetGameplayModule : NetworkBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    
    private NetworkedBridge _bridge;
    private readonly SyncVar<float> _health = new();
    private readonly SyncVar<PlayerID> _playerID = new();
    
    public float Health => _health.Value;
    public PlayerID PlayerID => _playerID.Value;
    
    public void ServerInit(NetworkedBridge bridge, PlayerID playerID)
    {
        _bridge = bridge;
        _bridge.AddModuleBaseStats(ModuleID.GetModuleData().BaseStats);
        _health.Value = ModuleID.GetModuleData().BaseStats.health;
        _playerID.Value = playerID;
    }

    public override void OnStartClient()
    {
        _bridge = ModuleID == NetModuleID.Bridge ? GetComponent<NetworkedBridge>() : GetComponentInParent<NetworkedBridge>();
        VisualTransform.SetParent(_bridge.VisualRootTransform);
    }

    private void DetachModule()
    {
        _bridge.DetachModuleBaseStats(ModuleID.GetModuleData().BaseStats);
        DetachModuleObserver();
    }

    [ObserversRpc]
    public void DetachModuleObserver()
    {
        Destroy(VisualTransform);
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
