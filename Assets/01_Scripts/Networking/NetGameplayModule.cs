using _01_Scripts.Ship;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetGameplayModule : NetworkBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }

    [SerializeField] private GameObject deathVFX;

    [Header("Detachment Settings")] [SerializeField]
    private float _detachmentForce;

    private NetBridge _bridge;
    private readonly SyncVar<float> _health = new();
    private readonly SyncVar<NetPlayerID> _playerID = new();
    
    // HexCoordinate relative to attached bridge coordinate
    private readonly SyncVar<HexCoordinate> _rootCoordinate = new();

    public float Health => _health.Value;
    public NetPlayerID NetPlayerID => _playerID.Value;
    public HexCoordinate RootCoordinate => _rootCoordinate.Value;

    public void S_ServerInit(NetBridge bridge, NetPlayerID netPlayerID, HexCoordinate rootCoordinate)
    {
        _bridge = bridge;
        _rootCoordinate.Value = rootCoordinate;
        _bridge.S_AttachModule(this, rootCoordinate);
        _health.Value = ModuleID.GetModuleData().BaseStats.health;
        _playerID.Value = netPlayerID;
    }

    public override void OnStartClient()
    {
        _bridge = ModuleID == NetModuleID.Bridge ? GetComponent<NetBridge>() : GetComponentInParent<NetBridge>();
        VisualTransform.SetParent(_bridge.VisualRootTransform);
    }

    // Occurs when a module gets destroyed
    private void S_DestroyModule()
    {
        _bridge.S_DetachModule(this, _rootCoordinate.Value);
        _bridge.DetachLooseModules();
        C_DestroyModuleObserver();
        Despawn(NetworkObject);
    }
    
    // Occurs when an Module is only detached and not destroyed
    public void S_DetachModule()
    {
        _bridge.S_DetachModule(this, _rootCoordinate.Value);
        C_DetachModuleObserver();
        Despawn(NetworkObject);
    }

    [ObserversRpc]
    public void C_DestroyModuleObserver()
    {
        if (deathVFX != null)
        {
            Instantiate(deathVFX, VisualTransform.position, Quaternion.identity);
        }

        Destroy(VisualTransform.gameObject);
    }

    [ObserversRpc]
    public void C_DetachModuleObserver()
    {
        Vector2 detachDirection = (VisualTransform.position - _bridge.transform.position).normalized;
        DetachedModuleSpawner.Instance.SpawnDetachedModule(ModuleID, VisualTransform, detachDirection * 10f);

        Destroy(VisualTransform.gameObject);
    }

    public void S_InflictDamage(float damage)
    {
        _health.Value -= damage;
        if (_health.Value <= 0)
        {
            S_DestroyModule();
        }
    }
}