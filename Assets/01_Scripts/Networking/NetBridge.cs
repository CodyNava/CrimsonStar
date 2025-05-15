using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetBridge : NetworkBehaviour
{
    [field: SerializeField] public NetBridgeConfig BridgeConfig { get; private set; }
    [field: SerializeField] public HexTransform HexTransform { get; private set; }
    [field: SerializeField] public Transform VisualRootTransform { get; private set; }

    private readonly SyncVar<NetModuleBaseStats> _baseStats = new();
    public NetModuleBaseStats BaseStats => _baseStats.Value;

    private Dictionary<HexCoordinate, NetGameplayModule> _modules = new();
    public NetGameplayModule BridgeModule => _modules[HexCoordinate.Zero];

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
            foreach (var (c,m) in modules)
            {
                m.S_DetachModule();
            }
            
            InstanceFinder.GetInstance<NetGameplayConductor>().S_RegisterPlayerDeath();
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
            Assert.False(_modules.ContainsKey(localHexCoordinate + rootCoordinate), "Placement check failed! Tried to add Module that overlaps already occupied HexCoordinate!");
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
            if(module.ModuleID == NetModuleID.Bridge) continue; 
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
            if(handledCoordinates.Contains(coord)) continue;
            
            // We add the to checking coord to handled coords, so no loop would occur on checking modules next to each other
            handledCoordinates.Add(coord);
            
            // If the coord is empty, do nothing
            if(!_modules.ContainsKey(coord)) continue;
            
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
}
