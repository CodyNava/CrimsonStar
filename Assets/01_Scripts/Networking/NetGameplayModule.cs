using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Ship;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FMOD.Studio;
using FMODUnity;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class NetGameplayModule : NetworkBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    [field: SerializeField] public int WeaponGroup { get; set; }

    [SerializeField] private GameObject deathVFX;
    [SerializeField] private VisualEffect damagedVFX;
    [SerializeField] private MeshRenderer damagedMaterial;

    [SerializeField] private EventReference hitFeedbackSFX;
    [SerializeField] private EventReference gotHitFeedbackSFX;
    [SerializeField] private EventReference bridgeGotHitFeedbackSFX;
    [SerializeField] private EventReference lowHealthAlarmSFX;
    [SerializeField] private Material glassCrackedMaterial;
    private EventInstance _lowHealthAlarmInstance;

    [Header("ColorPresets")] private static readonly int Shift = Shader.PropertyToID("_ColourShift");
    private static readonly int BridgeHealthInput = Shader.PropertyToID("BridgeHealthInput");
    [field: SerializeField] private Vector4 PresetColor1 { get; set; }
    [field: SerializeField] private Vector4 PresetColor2 { get; set; }
    [field: SerializeField] private Vector4 PresetColor3 { get; set; }
    [field: SerializeField] private Material PresetMat1 { get; set; }
    [field: SerializeField] private Material PresetMat2 { get; set; }
    [field: SerializeField] private Material PresetMat3 { get; set; }
    [field: SerializeField] private Material PresetMatHead1 { get; set; }
    [field: SerializeField] private Material PresetMatHead2 { get; set; }
    [field: SerializeField] private Material PresetMatHead3 { get; set; }
    [field: SerializeField] private GameObject PresetObject { get; set; }
    [field: SerializeField] private GameObject PresetObjectHead { get; set; }
    [field: SerializeField] private ColorPresetData PresetData { get; set; }

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
    public void S_ServerInit(NetBridge bridge, NetTeamID netTeamID, HexCoordinate rootCoordinate, string presetName)
    {
        var moduleData = ModuleID.GetModuleData();

        _bridge = bridge;
        _rootCoordinate.Value = rootCoordinate;
        _health.Value = moduleData.BaseStats.health;
        _maxHealth = _health.Value;
        _playerID.Value = netTeamID;
        _bridge.S_AttachModule(this, rootCoordinate);
        C_SetColorsBasedOnPreset(presetName);
    }

    public void C_ClientInit()
    {
    }

    public void OnDestroy()
    {
        if (ModuleID != NetModuleID.Bridge) return;
        _lowHealthAlarmInstance.stop(STOP_MODE.IMMEDIATE);
        _lowHealthAlarmInstance.release();
        glassCrackedMaterial.SetFloat(BridgeHealthInput, 0f);
    }

    public override void OnStartClient()
    {
        _bridge = ModuleID == NetModuleID.Bridge ? GetComponent<NetBridge>() : GetComponentInParent<NetBridge>();
        var coord = _bridge.HexTransform.Layout.PositionXYToHex(transform.position);
        Debug.Log($"IsClient: {IsClientStarted}, PlayerID: {_playerID.Value}, Module: {ModuleID}, Bridge: {_bridge}");
        var moduleData = ModuleID.GetModuleData();
        _maxHealth = moduleData.BaseStats.health;
        VisualTransform.SetParent(_bridge.VisualRootTransform);
        if (lowHealthAlarmSFX.IsNull == false)
        {
            _lowHealthAlarmInstance = RuntimeManager.CreateInstance(lowHealthAlarmSFX);
        }

        GetPresetMaterials();
        // UpdateMaterialPresets();

        if (IsOwner)
        {
            //UpdateMaterialPresets();
            int weaponGroupValue = NetModuleWeaponGroupData.WeaponGroupMap.GetValueOrDefault(coord);
            WeaponGroup = weaponGroupValue;
        }
    }

    public void GetPresetMaterials()
    {
        PresetMat1 = PresetObject.GetComponent<MeshRenderer>().materials[0];
        PresetMat2 = PresetObject.GetComponent<MeshRenderer>().materials[1];
        PresetMat3 = PresetObject.GetComponent<MeshRenderer>().materials[2];
        if (!PresetObjectHead) return;
        PresetMatHead1 = PresetObjectHead.GetComponent<MeshRenderer>().materials[0];
        PresetMatHead2 = PresetObjectHead.GetComponent<MeshRenderer>().materials[1];
        PresetMatHead3 = PresetObjectHead.GetComponent<MeshRenderer>().materials[2];
    }
    
    [ObserversRpc]
    public void C_SetColorsBasedOnPreset(string presetName)
    {
        var currenPreset = DataProvider.GetColorPresetByName(presetName);
        PresetColor1 = currenPreset.color1;
        PresetColor2 = currenPreset.color2;
        PresetColor3 = currenPreset.color3;
        UpdateMaterialPresets();
    }

    private void UpdateMaterialPresets()
    {
        PresetMat1.SetVector(Shift, PresetColor1);
        PresetMat2.SetVector(Shift, PresetColor2);
        PresetMat3.SetVector(Shift, PresetColor3);
        if (PresetMatHead1 && PresetMatHead2 && PresetMatHead3)
        {
            PresetMatHead1.SetVector(Shift, PresetColor1);
            PresetMatHead2.SetVector(Shift, PresetColor2);
            PresetMatHead3.SetVector(Shift, PresetColor3);
        }
    }
    

    
    [Server]
    private void SetHealth(float value) => _health.Value -= value;
    
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
            if (ModuleID == NetModuleID.Reactor)
            {
                //todo implement reactor explosion Here
            }
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
    public void C_DisplayDamageObserver(float HealthPct)
    {
        float health = HealthPct;
        Debug.Log("AAAAAA " + _health + "BBBBB" + HealthPct);
        damagedVFX.SetFloat("DamageInput", 1 - health);
        damagedMaterial.material.SetFloat("_InputHealth", 1 - health);
        if (IsOwner)
        {
            if (ModuleID == NetModuleID.Bridge)
            {
                glassCrackedMaterial.SetFloat(BridgeHealthInput, 1 - health);
                RuntimeManager.PlayOneShot(bridgeGotHitFeedbackSFX, transform.position);
                if (lowHealthAlarmSFX.IsNull == false)
                {
                    if (_health.Value <= _maxHealth * 0.35f)
                    {
                        _lowHealthAlarmInstance.getPlaybackState(out PLAYBACK_STATE state);
                        if (state == PLAYBACK_STATE.STOPPED)
                            _lowHealthAlarmInstance.start();
                    }
                }
            }
            else
            {
                RuntimeManager.PlayOneShot(gotHitFeedbackSFX, transform.position);
            }
        }
    }

    [Server]
    [ServerRpc(RequireOwnership = false)]
    public void S_InflictDamage(float damage, ulong attackerID = 0)
    {
        if (InstanceFinder.TryGetInstance(out NetGameplayConductor gameplayConductor) && attackerID != 0)
        {
            gameplayConductor.S_ReportDamageInstance(attackerID, _bridge.PlayerID, damage);
        }

        SetHealth(damage);
        if (_health.Value <= 0)
        {
            if (lowHealthAlarmSFX.IsNull == false)
            {
                _lowHealthAlarmInstance.stop(STOP_MODE.IMMEDIATE);
                _lowHealthAlarmInstance.release();

                if (this.ModuleID != NetModuleID.Bridge) return;
                glassCrackedMaterial.SetFloat(BridgeHealthInput, 0f);
            }

            if (ModuleID == NetModuleID.Bridge && gameplayConductor && attackerID != 0)
            {
                gameplayConductor.S_ReportKillInstance(attackerID, _bridge.PlayerID);
            }

            S_DestroyModule();
        }

        Debug.Log("damage inflicted: " + damage);
        float newPct = Mathf.Clamp01(_health.Value / Mathf.Max(_maxHealth, Mathf.Epsilon)); // direkt die healthpct übergeben weil die syncticks zu langsam sind 
        C_DisplayDamageObserver(newPct);
    }
}