using _01_Scripts.Ship;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public float Health => _health.Value;
    public NetPlayerID NetPlayerID => _playerID.Value;

    public void S_ServerInit(NetBridge bridge, NetPlayerID netPlayerID)
    {
        _bridge = bridge;
        _bridge.S_AttachModule(this);
        _health.Value = ModuleID.GetModuleData().BaseStats.health;
        _playerID.Value = netPlayerID;
    }

    public override void OnStartClient()
    {
        _bridge = ModuleID == NetModuleID.Bridge ? GetComponent<NetBridge>() : GetComponentInParent<NetBridge>();
        VisualTransform.SetParent(_bridge.VisualRootTransform);
    }

    private void S_DetachModule()
    {
        _bridge.S_DetachModule(this);
        C_DetachModuleObserver();
        Despawn(NetworkObject);
    }

    [ObserversRpc]
    public void C_DetachModuleObserver()
    {
        if (deathVFX != null)
        {
            Instantiate(deathVFX, VisualTransform.position, Quaternion.identity);
            
            // TODO: Debug detachment of destroyed module to test detachment.
            //  Needs to be moved into method to spawn for true detached modules
            Vector2 detachDirection = (VisualTransform.position - _bridge.transform.position).normalized;
            DetachedModuleSpawner.Instance.SpawnDetachedModule(ModuleID, VisualTransform, detachDirection * _detachmentForce);
        }

        Destroy(VisualTransform.gameObject);
    }

    public void S_InflictDamage(float damage)
    {
        _health.Value -= damage;
        if (_health.Value <= 0)
        {
            S_DetachModule();
        }
    }
}