using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using _01_Scripts.Ship;
using _01_Scripts.Ship.ModuleControllers;
using FishNet.Component.Prediction;
using FishNet.Connection;
using FishNet.Transporting;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class NetBridge : NetworkBehaviour
{
    [field: SerializeField] public NetBridgeConfig BridgeConfig { get; private set; }
    [field: SerializeField] public HexTransform HexTransform { get; private set; }
    [field: SerializeField] public Transform VisualRootTransform { get; private set; }

    [SerializeField] private GameObject deathVFX;
    private readonly SyncVar<NetModuleBaseStats> _baseStats = new();
    private readonly SyncVar<string> _displayName = new();
    private readonly SyncVar<ulong> _playerId = new();
    public CameraZoom CameraZoom { get; private set; }
    public CameraFollow CameraFollow { get; private set; }
    public NetModuleBaseStats BaseStats => _baseStats.Value;
    public string DisplayName => _displayName.Value;
    public ulong PlayerID => _playerId.Value;

    private Dictionary<HexCoordinate, NetGameplayModule> _modules = new();
    private Dictionary<HexCoordinate, int> _powerGrid = new();
    public Dictionary<HexCoordinate, int> PowerGrid => _powerGrid;
    public NetGameplayModule BridgeModule => _modules[HexCoordinate.Zero];

    private NetworkCollision2D _networkCollision2D;

    [SerializeField] private StudioListener fmodListener;

    [Server]
    public void S_AttachModule(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        _baseStats.Value = _baseStats.Value.Combine(module.ModuleID.GetModuleData().BaseStats);
        S_AddModuleCoordinates(module, rootCoordinate);
    }

    [Server]
    public void S_DetachModule(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        _baseStats.Value = _baseStats.Value.Subtract(module.ModuleID.GetModuleData().BaseStats);
        S_RemoveModuleCoordinates(module, rootCoordinate);

        if (module.ModuleID == NetModuleID.Bridge)
        {
            Dictionary<HexCoordinate, NetGameplayModule> modules =
                new Dictionary<HexCoordinate, NetGameplayModule>(_modules);
            foreach (var (c, m) in modules)
            {
                m.S_DetachModule();
            }

            InstanceFinder.GetInstance<NetGameplayConductor>().S_RegisterPlayerDeath(NetworkObject.Owner);
            Despawn(NetworkObject);
        }
    }

    [Server]
    public void S_DetachLooseModules()
    {
        var looseModules = GetLooseModules();
        foreach (NetGameplayModule looseModule in looseModules)
        {
            looseModule.S_DetachModule();
        }
    }

    [Server]
    private void S_AddModuleCoordinates(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        var moduleData = module.ModuleID.GetModuleData();
        var localHexCoordinates = moduleData.GetLocalHexCoordinates();
        foreach (HexCoordinate localHexCoordinate in localHexCoordinates)
        {
            HexCoordinate coordinate = localHexCoordinate + rootCoordinate;
            Assert.IsFalse(_modules.ContainsKey(coordinate),
                "Placement check failed! Tried to add Module that overlaps already occupied HexCoordinate!");
            // We add each localHexCoordinate that the module occupies to the list
            // As the localHexCoordinates are only in module local space, we add the rootCoordinate as an offset
            _modules.Add(coordinate, module);
        }

        if (module.ModuleID == NetModuleID.Reactor)
        {
            foreach (var coordinate in rootCoordinate.CoordinatesInRange(moduleData.EffectRange))
            {
                int power = _powerGrid.GetValueOrDefault(coordinate);
                _powerGrid[coordinate] = power + 1;
            }

            C_AddToPowerGrid(rootCoordinate, moduleData.EffectRange);
        }
    }

    [ObserversRpc]
    [Client]
    private void C_AddToPowerGrid(HexCoordinate rootCoordinate, int range)
    {
        foreach (var coordinate in rootCoordinate.CoordinatesInRange(range))
        {
            int power = _powerGrid.GetValueOrDefault(coordinate);
            _powerGrid[coordinate] = power + 1;
        }
    }
    [Server]
    private void S_RemoveModuleCoordinates(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        var moduleData = module.ModuleID.GetModuleData();
        var localHexCoordinates = moduleData.GetLocalHexCoordinates();
        foreach (HexCoordinate localHexCoordinate in localHexCoordinates)
        {
            HexCoordinate coordinate = localHexCoordinate + rootCoordinate;
            // We remove each localHexCoordinate that the module occupies to the list
            // As the localHexCoordinates are only in module local space, we add the rootCoordinate as an offset
            _modules.Remove(coordinate);
        }

        if (module.ModuleID == NetModuleID.Reactor)
        {
            foreach (var coordinate in rootCoordinate.CoordinatesInRange(2))
            {
                int power = _powerGrid.GetValueOrDefault(coordinate);
                _powerGrid[coordinate] = power - 1;
            }

            C_RemovePowerFromGrid(rootCoordinate, moduleData.EffectRange);
        }
    }

    [ObserversRpc]
    [Client]
    private void C_RemovePowerFromGrid(HexCoordinate rootCoordinate, int range)
    {
        foreach (HexCoordinate coordinate in rootCoordinate.CoordinatesInRange(range))
        {
            int power = _powerGrid.GetValueOrDefault(coordinate);
            _powerGrid[coordinate] = power - 1;
        }
    }

    // TODO: Check performance impact and might need to be optimized to reduce call count
    private HashSet<NetGameplayModule> GetLooseModules()
    {
        // Copy all modules into an emptying hashset
        HashSet<NetGameplayModule> looseModules = new HashSet<NetGameplayModule>();
        foreach (var (coord, module) in _modules)
        {
            if (module.ModuleID == NetModuleID.Bridge) continue;
            looseModules.Add(module);
        }

        // If the Bridge isn't present anymore, everything else is a loose module
        if (!_modules.ContainsKey(HexCoordinate.Zero)) return looseModules;


        HashSet<HexCoordinate> handledCoordinates = new HashSet<HexCoordinate>();
        Queue<HexCoordinate> checkingCoordinates = new Queue<HexCoordinate>();

        // Initialize Queue with NeighbourCoordinates of Bridge
        handledCoordinates.UnionWith(BridgeModule.ModuleID.GetModuleData().GetLocalHexCoordinates());
        var bridgeNeighbourCoordinates = BridgeModule.ModuleID.GetModuleData().GetLocalNeighbourCoordinates();
        foreach (HexCoordinate bridgeNeighbourCoordinate in bridgeNeighbourCoordinates)
        {
            checkingCoordinates.Enqueue(bridgeNeighbourCoordinate);
        }


        while (checkingCoordinates.TryDequeue(out HexCoordinate coord))
        {
            // We might have added the to checking coord already as handled, as an earlier coord pointed to the same module
            if (handledCoordinates.Contains(coord)) continue;

            // We add the to checking coord to handled coords, so no loop would occur on checking modules next to each other
            handledCoordinates.Add(coord);

            // If the coord is empty, do nothing
            if (!_modules.ContainsKey(coord)) continue;

            NetGameplayModule module = _modules[coord];
            looseModules.Remove(module);

            // TODO: Rotated modules might need more care to get the correct occupied coordinates
            // We add all occupied coords of the module to handled coords
            var moduleOwnCoords = module.ModuleID.GetModuleData().GetLocalHexCoordinates();
            foreach (HexCoordinate ownCoord in moduleOwnCoords)
            {
                handledCoordinates.Add(ownCoord + module.RootCoordinate);
            }

            // TODO: Rotated modules might need more care to get the correct valid neighbour coordinates
            // We add all neighbouring coords of the module to the checking list, that weren't handled already
            var moduleNeighbourCoords = module.ModuleID.GetModuleData().GetLocalNeighbourCoordinates();
            foreach (HexCoordinate ownCoord in moduleNeighbourCoords)
            {
                if (handledCoordinates.Contains(ownCoord + module.RootCoordinate)) continue;

                checkingCoordinates.Enqueue(ownCoord + module.RootCoordinate);
            }
        }

        return looseModules;
    }

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            CameraZoom = FindFirstObjectByType<CameraZoom>();
            CameraFollow = FindFirstObjectByType<CameraFollow>();
            CameraFollow.SetTarget(this);
            fmodListener.enabled = true;
        }
        else
        {
            fmodListener.enabled = false;
        }
    }

    public override void OnStopClient()
    {
        if (IsOwner && !CameraFollow.IsUnityNull()) CameraFollow.SetTarget(null);

        Instantiate(deathVFX, VisualRootTransform.position, Quaternion.identity);
        Destroy(VisualRootTransform.gameObject);
    }

    public float ComputeRotationSpeed()
    {
        return BridgeConfig.BaseAngularSpeed + _baseStats.Value.angularThrust / (1 + _baseStats.Value.mass);
    }

    public float ComputeMovementSpeed()
    {
        return BridgeConfig.BaseMovementSpeed + _baseStats.Value.thrust / (1 + _baseStats.Value.mass);
    }

    public float GetAngularDampingCoefficient()
    {
        return BridgeConfig.AngularDampingCoefficient;
    }

    public float GetLinearDampingCoefficient()
    {
        return BridgeConfig.LinearDampingCoefficient;
    }

    public float GetMaxMoveSpeed()
    {
        return BridgeConfig.MaxMovementSpeed / (1 + _baseStats.Value.mass);
    }

    public float GetMaxAngularVelocity()
    {
        return BridgeConfig.MaxAngularSpeed / (1 + _baseStats.Value.mass);
    }

    [Server]
    public void S_SetDisplayName(string displayName)
    {
        _displayName.Value = displayName;
    }

    [Server]
    public void S_SetPlayerID(ulong playerID)
    {
        _playerId.Value = playerID;
    }

    [ObserversRpc(ExcludeOwner = false)]
    public void HandleEndOfRound()
    {
        // Todo: Perhaps replace ship with non-networked copy and despawn networked version w/o playing explosions
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        OnEnterCollision2D(collision.collider);
    }


    private void OnEnterCollision2D(Collider2D collider)
    {
        // Only the server should handle collision damage
        if (!IsServerInitialized) return;

        NetGameplayModule module = collider.gameObject.GetComponent<NetGameplayModule>();
        BaseModuleController moduleController = collider.gameObject.GetComponent<BaseModuleController>();

        if (module == null && moduleController == null) return;

        if (module == null || module.Bridge != this)
        {
            S_HandleCollision(collider);
        }
    }

    [Server]
    private void S_HandleCollision(Collider2D collider)
    {
        // TODO: Make magic number not magic anymore
        float kineticEnergyConstant = 1f;
        float velocityThreshold = 1f;
        float impactEnergyModifier = 3.5f;

        ContactPoint2D[] contacts = new ContactPoint2D[1];
        if (collider.GetContacts(contacts) < 1) return;

        ContactPoint2D contactPoint = contacts[0];
        Vector2 relVel = -contactPoint.relativeVelocity;

        if (relVel.magnitude < velocityThreshold) return;


        float massA = BaseStats.mass;
        Vector2 impactNormal = contactPoint.normal;

        Rigidbody2D localBody2D = contactPoint.rigidbody;
        Rigidbody2D remoteBody2D = contactPoint.otherRigidbody;
        Collider2D localCollider = contactPoint.collider;

        // Currently disabled, as the damage calculation doesnt accounts for ship alignment toward impact normal
        // float dotA = Mathf.Abs(Vector2.Dot(localBody2D.linearVelocity.normalized, impactNormal));

        NetGameplayModule otherGameplayModule = remoteBody2D.gameObject.GetComponent<NetGameplayModule>();
        if (otherGameplayModule == null || otherGameplayModule.Bridge == this) return;
        float massB = otherGameplayModule.Bridge.BaseStats.mass;

        // Energy calculations
        float impactEnergy = impactEnergyModifier * kineticEnergyConstant * (massA * massB / (massA + massB)) *
                             relVel.sqrMagnitude;

        // Damage calculations
        float damage = impactEnergy * (massB / (massA + massB));

        NetGameplayModule gameplayModule = localCollider.gameObject.GetComponent<NetGameplayModule>();

        // TODO: Probably causes issues on damageDealt, which causes Dealt and Received not to align
        gameplayModule.S_InflictDamage(damage, _playerId.Value);
    }

    public bool PositionHasEnergy(HexCoordinate coord)
    {
        return _powerGrid.GetValueOrDefault(coord) > 0;
    }
}