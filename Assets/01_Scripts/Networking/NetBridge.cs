using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using _01_Scripts.Ship.ModuleControllers;
using FishNet.Component.Prediction;
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
    public NetModuleBaseStats BaseStats => _baseStats.Value;
    public string DisplayName => _displayName.Value;

    private Dictionary<HexCoordinate, NetGameplayModule> _modules = new();
    public NetGameplayModule BridgeModule => _modules[HexCoordinate.Zero];


    private NetworkCollision2D _networkCollision2D;
    private void Awake()
    {
         // _networkCollision2D = gameObject.GetComponent<NetworkCollision2D>();
         //
         // _networkCollision2D.OnEnter += OnEnterCollision2D;
    }

    public void OnDestroy()
    {
         // if(_networkCollision2D != null)
         //     _networkCollision2D.OnEnter -= OnEnterCollision2D;
    }

    public void S_AttachModule(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        _baseStats.Value = _baseStats.Value.Combine(module.ModuleID.GetModuleData().BaseStats);
        module.ModuleID.GetModuleData().GetLocalHexCoordinates();
        AddModuleCoordinates(module, rootCoordinate);
    }

    public void S_DetachModule(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        _baseStats.Value = _baseStats.Value.Subtract(module.ModuleID.GetModuleData().BaseStats);
        RemoveModuleCoordinates(module, rootCoordinate);

        if (module.ModuleID == NetModuleID.Bridge)
        {
            Dictionary<HexCoordinate, NetGameplayModule> modules = new Dictionary<HexCoordinate, NetGameplayModule>(_modules);
            foreach (var (c, m) in modules)
            {
                m.S_DetachModule();
            }

            InstanceFinder.GetInstance<NetGameplayConductor>().S_RegisterPlayerDeath(NetworkObject.Owner);
            Despawn(NetworkObject);
        }
    }

    public void S_DetachLooseModules()
    {
        var looseModules = GetLooseModules();
        foreach (NetGameplayModule looseModule in looseModules)
        {
            looseModule.S_DetachModule();
        }
    }

    private void AddModuleCoordinates(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        var localHexCoordinates = module.ModuleID.GetModuleData().GetLocalHexCoordinates();
        foreach (HexCoordinate localHexCoordinate in localHexCoordinates)
        {
            Assert.IsFalse(_modules.ContainsKey(localHexCoordinate + rootCoordinate), "Placement check failed! Tried to add Module that overlaps already occupied HexCoordinate!");
            // We add each localHexCoordinate that the module occupies to the list
            // As the localHexCoordinates are only in module local space, we add the rootCoordinate as an offset
            _modules.Add(localHexCoordinate + rootCoordinate, module);
        }
    }

    private void RemoveModuleCoordinates(NetGameplayModule module, HexCoordinate rootCoordinate)
    {
        var localHexCoordinates = module.ModuleID.GetModuleData().GetLocalHexCoordinates();
        foreach (HexCoordinate localHexCoordinate in localHexCoordinates)
        {
            // We remove each localHexCoordinate that the module occupies to the list
            // As the localHexCoordinates are only in module local space, we add the rootCoordinate as an offset
            _modules.Remove(localHexCoordinate + rootCoordinate);
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
            FindFirstObjectByType<CameraFollow>().SetTargetFollow(VisualRootTransform);
        }
    }
    public override void OnStopClient()
    {
        if (IsOwner)
        {
            FindFirstObjectByType<CameraFollow>()?.SetTargetFollow(null);
        }
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

    public void S_SetDisplayName(string displayName)
    {
        _displayName.Value = displayName;
    }

    [ObserversRpc(ExcludeOwner = false)]
    public void HandleEndOfRound()
    {
        // Todo: Perhaps replace ship with non-networked copy and despawn networked version w/o playing explosions
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Unity] Collided with {collision.collider.name}");
        OnEnterCollision2D(collision.collider);
    }


    private void OnEnterCollision2D(Collider2D collider)
    {
        if (!IsServerInitialized) return;
        
        Debug.Log("Collision detected!");
        NetGameplayModule module = collider.gameObject.GetComponent<NetGameplayModule>();
        BaseModuleController moduleController = collider.gameObject.GetComponent<BaseModuleController>();

        if (module == null && moduleController == null) return;
        
        if (module == null || module.Bridge != this)
        {
            HandleCollision(collider);
        }
    }

    private void HandleCollision(Collider2D collider)
    {
        // Self perspective always ship A and other is ship B
        // Get sum of Masses of own Ship and other Ship
        // Get relative velocity between both ships along the collisionNormal
        // Get facing direction of both ships relative to collisionNormal
        // Calculate impactEnergy
        // Calculate own impactDamage to be applied to collided own module

        // TODO: Make magic number not magic anymore
        float kineticEnergyConstant = 10f;
        float velocityThreshold = 1f;

        ContactPoint2D[] contacts = new ContactPoint2D[1];
        if (collider.GetContacts(contacts) < 1) return;
        
        ContactPoint2D contactPoint = contacts[0];
        Vector2 relVel = -contactPoint.relativeVelocity;

        Debug.Log($"{relVel.magnitude}");
        if (relVel.magnitude < velocityThreshold)
        {
            Debug.Log($"too slow! Sucker!!");
            return;
        }

        float massA, massB;
        Vector2 impactNormal;
        float dotA, dotB;
        float impactEnergy;

        massA = BaseStats.mass;
        impactNormal = relVel.normalized;
        
        Rigidbody2D localBody2D = contactPoint.rigidbody;
        Rigidbody2D remoteBody2D = contactPoint.otherRigidbody;
        Collider2D localCollider = contactPoint.collider;
        Collider2D remoteCollider = contactPoint.otherCollider;
        Debug.Log($"LocalBody: {localBody2D.gameObject.name}");
        Debug.Log($"RemoteBody: {remoteBody2D.gameObject.name}");
        Debug.Log($"LocalCollider: {localCollider.gameObject.name}");
        Debug.Log($"RemoteCollider: {remoteCollider.gameObject.name}");

        dotA = Vector2.Dot(localBody2D.linearVelocity, impactNormal);

        Debug.Log($"RelativeVelocity: {relVel}");
        Debug.DrawLine(contactPoint.point, contactPoint.point + impactNormal, Color.cyan, 10f);

        NetGameplayModule otherGameplayModule = remoteBody2D.gameObject.GetComponent<NetGameplayModule>();
        // In production this case shouldnt fail, only in test setup with collision of dummyEnemy
        if (otherGameplayModule != null)
        {
            // We collided with a Networked player
            
            // For some reason we collided with ourself
            if (otherGameplayModule.Bridge == this) return;
            
            massB = otherGameplayModule.Bridge.BaseStats.mass;
        }
        else
        {
            BaseModuleController moduleController = remoteBody2D.gameObject.GetComponent<BaseModuleController>();
            massB = moduleController.ShipController.BridgeController.Mass;
        }

        // Energy calculations
        impactEnergy = kineticEnergyConstant * (massA * massB / (massA + massB)) * relVel.sqrMagnitude;
        Debug.Log($"ImpactEnergy: {impactEnergy}");
        
        // Damage calculations
        float damage = impactEnergy * (massB / (massA + massB)) *
                       (1 - kineticEnergyConstant * Mathf.Max(dotA, 0f));

        Debug.Log($"Damage: {damage} = {impactEnergy} * ({massB} / ({massA} + {massB})) * (1 - {kineticEnergyConstant} * {Mathf.Max(dotA, 0)}");
        
        NetGameplayModule gameplayModule = localCollider.gameObject.GetComponent<NetGameplayModule>();
        gameplayModule.S_InflictDamage(damage, SteamPlayer.SteamID);
    }
}
