using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModulePlacementData
{
    public HexCoordinate RootCoordinate = HexCoordinate.Zero;
    public int Rotation;
    public NetModuleID ModuleID;
}

public class NetModuleStorage : NetworkBehaviour
{
    private static readonly HexCoordinate[] BridgeCoordinates = {
        HexCoordinate.Zero,
        HexCoordinate.Neighbor(HexCoordinate.Zero, HexDirection.North),
    };

    private readonly SyncDictionary<HexCoordinate, ModulePlacementData> _moduleMap = new();

    public void Init()
    {
        _moduleMap.Clear();
        var bridgeData = new ModulePlacementData
        {
            RootCoordinate = HexCoordinate.Zero,
            Rotation = 0,
            ModuleID = NetModuleID.Bridge
        };
        foreach (HexCoordinate bridgeCoord in BridgeCoordinates)
        {
            _moduleMap[bridgeCoord] = bridgeData;
        }
    }

    public void C_AddModule(HexCoordinate coord, NetModuleID id, int rotation)
    {
        if (IsOwner)
        {
            S_AddModule(coord, id, rotation);
        }
    }
    
    public void C_RemoveModule(HexCoordinate coord)
    {
        if (IsOwner)
        {
            S_RemoveModule(coord);
        }
    }
    
    public bool SC_IsCoordinateOccupied(HexCoordinate coord)
    {
        return _moduleMap.ContainsKey(coord);
    }

    public bool SC_IsNeighboringModule(HexCoordinate coord)
    {
        return coord.Neighbors().Any(SC_IsCoordinateOccupied);
    }
    
    public IEnumerable<ModulePlacementData> GetUniqueModules()
    {
        HashSet<HexCoordinate> spawnedRoots = new();
        foreach (ModulePlacementData placementData in _moduleMap.Values)
        {
            if (!spawnedRoots.Add(placementData.RootCoordinate)) continue;
            if (placementData.ModuleID <= NetModuleID.Bridge) continue;
            yield return placementData;
        }
    }
    
    [ServerRpc]
    private void S_AddModule(HexCoordinate coord, NetModuleID id, int rotation)
    {
        ModulePlacementData placementData = new()
        {
            RootCoordinate = coord,
            Rotation = rotation,
            ModuleID = id
        };

        // GetModuleData() is an extension method on ModuleID returning its entry in DataProvider.Instance.ModuleDB.ModuleData
        var localHexCoordinates = id.GetModuleData().GetLocalHexCoordinates();
        
        foreach (var rotatedLocalCoord in GetRotatedCoordinates(localHexCoordinates, rotation))
        {
           S_AddModuleReference(coord + rotatedLocalCoord, placementData);
        }
    }
    
    [ServerRpc]
    private void S_RemoveModule(HexCoordinate coord)
    {
        if (!_moduleMap.TryGetValue(coord, out ModulePlacementData placementData))
        {
            Debug.LogError($"Tried remove module at Coordinate ({coord.Q}, {coord.R}, {coord.S}) which was not found.");
            return;
        }
        var localHexCoordinates = placementData.ModuleID.GetModuleData().GetLocalHexCoordinates();
        foreach (var rotatedLocalCoord in GetRotatedCoordinates(localHexCoordinates, placementData.Rotation))
        {
            S_RemoveModuleReference(placementData.RootCoordinate + rotatedLocalCoord);
        
        }
        
    }

    public static IEnumerable<HexCoordinate> GetRotatedCoordinates(IEnumerable<HexCoordinate> coords, int rotationCount)
    {
        foreach (HexCoordinate coord in coords)
        {
            HexCoordinate rotatedCoord = coord;
            for (int i = 0; i < rotationCount; i++)
            {
                rotatedCoord = rotatedCoord.RotateClockwise();
            }
            
            yield return rotatedCoord;
        }
    }

    private void S_AddModuleReference(HexCoordinate coord, ModulePlacementData data)
    {
        _moduleMap[coord] = data;
    }

    private void S_RemoveModuleReference(HexCoordinate coord)
    {
        _moduleMap.Remove(coord);
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !IsOwner) return;
        foreach (HexCoordinate coord in _moduleMap.Keys)
        {
            coord.DrawGizmos(Color.yellow, 2f, 0.9f);
        }
    }
}
