using _01_Scripts.Ship;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FMOD.Studio;
using FMODUnity;
using Steamworks;
using UnityEngine;
using UnityEngine.VFX;

public class NetGameplayModule : NetworkBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }

    [SerializeField] private GameObject deathVFX;
    [SerializeField] private VisualEffect damagedVFX;
    [SerializeField] private MeshRenderer damagedMaterial;

    [SerializeField] private EventReference hitFeedbackSFX;
    [SerializeField] private EventReference gotHitFeedbackSFX;
    [SerializeField] private EventReference lowHealthAlarmSFX;
    private EventInstance _lowHealthAlarmInstance;

    [Header("Detachment Settings")] [SerializeField]
    private float _detachmentForce;

    private NetBridge _bridge;
    private float _maxHealth;
    private readonly SyncVar<float> _health = new();
    private readonly SyncVar<NetTeamID> _playerID = new();

    // HexCoordinate relative to attached bridge coordinate
    private readonly SyncVar<HexCoordinate> _rootCoordinate = new();

    public NetBridge Bridge => _bridge;
    public float Health => _health.Value;
    public float HealthPct => Mathf.Clamp01(Health / Mathf.Max(_maxHealth, Mathf.Epsilon));
    public NetTeamID NetTeamID => _playerID.Value;
    public HexCoordinate RootCoordinate => _rootCoordinate.Value;

    [Server]
    public void S_ServerInit(NetBridge bridge, NetTeamID netTeamID, HexCoordinate rootCoordinate)
    {
        var moduleData = ModuleID.GetModuleData();

        _bridge = bridge;
        _rootCoordinate.Value = rootCoordinate;
        _health.Value = moduleData.BaseStats.health;
        _maxHealth = _health.Value;
        _playerID.Value = netTeamID;
        _bridge.S_AttachModule(this, rootCoordinate);
    }

    public void C_ClientInit()
    {
    }

    public override void OnStartClient()
    {
        _bridge = ModuleID == NetModuleID.Bridge ? GetComponent<NetBridge>() : GetComponentInParent<NetBridge>();
        Debug.Log($"IsClient: {IsClientStarted}, PlayerID: {_playerID.Value}, Module: {ModuleID}, Bridge: {_bridge}");
        var moduleData = ModuleID.GetModuleData();
        _maxHealth = moduleData.BaseStats.health;
        VisualTransform.SetParent(_bridge.VisualRootTransform);
        if (lowHealthAlarmSFX.IsNull == false)
        {
            _lowHealthAlarmInstance = RuntimeManager.CreateInstance(lowHealthAlarmSFX);
        }
    }

    // Occurs when a module gets destroyed
    [Server]
    private void S_DestroyModule()
    {
        _bridge.S_DetachModule(this, _rootCoordinate.Value);
        _bridge.S_DetachLooseModules();
        C_DestroyModuleObserver();
        Despawn(NetworkObject);
    }

    // Occurs when an Module is only detached and not destroyed
    [Server]
    public void S_DetachModule()
    {
        _bridge.S_DetachModule(this, _rootCoordinate.Value);
        C_DetachModuleObserver();
        Despawn(NetworkObject);
    }

    [ObserversRpc]
    [Client]
    public void C_DestroyModuleObserver()
    {
        if (deathVFX != null)
        {
            Instantiate(deathVFX, VisualTransform.position, Quaternion.identity);
        }

        Destroy(VisualTransform.gameObject);
    }

    [ObserversRpc]
    [Client]
    public void C_DetachModuleObserver()
    {
        Vector2 detachDirection = (VisualTransform.position - _bridge.transform.position).normalized;
        DetachedModuleSpawner.Instance.SpawnDetachedModule(ModuleID, VisualTransform, detachDirection * 10f);
        Destroy(VisualTransform.gameObject);
    }

    [ObserversRpc]
    [Client]
    public void C_DisplayDamageObserver()
    {
        float health = HealthPct;

        damagedVFX.SetFloat("DamageInput", 1 - health);
        damagedMaterial.material.SetFloat("_InputHealth", 1 - health);
        if (IsOwner)
        {
            RuntimeManager.PlayOneShot(gotHitFeedbackSFX, transform.position);
            if (ModuleID == NetModuleID.Bridge && _health.Value <= _maxHealth * 0.75f)
            {
                _lowHealthAlarmInstance.start();
            }
        }
        else
        {
            RuntimeManager.PlayOneShot(hitFeedbackSFX, transform.position);
        }
        //Todo: Implement VFX Here
        // VFX Basierend auf healthPCT (VFX.INtensity = 1 - health) 
    }

    [Server]
    [ServerRpc(RequireOwnership = false)]
    public void S_InflictDamage(float damage, ulong attackerID)
    {
        if (InstanceFinder.TryGetInstance(out NetGameplayConductor gameplayConductor))
        {
            gameplayConductor.S_ReportDamageInstance(attackerID, _bridge.PlayerID, damage);
        }

        _health.Value -= damage;
        if (_health.Value <= 0)
        {
             _lowHealthAlarmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
             _lowHealthAlarmInstance.release();

            if (ModuleID == NetModuleID.Bridge && gameplayConductor)
            {
                gameplayConductor.S_ReportKillInstance(attackerID, _bridge.PlayerID);
            }

            S_DestroyModule();
        }

        C_DisplayDamageObserver();
    }
}