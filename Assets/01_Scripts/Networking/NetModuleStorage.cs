using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModulePlacementData
{
    public HexCoordinate RootCoordinate;
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
    
    public SyncDictionary<HexCoordinate, ModulePlacementData> ModuleMap => _moduleMap;

    private void Awake()
    {
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

    public void AddModule(HexCoordinate coord, NetModuleID id, int rotation)
    {
        if (IsOwner)
        {
            AddModuleRPC(coord, id, rotation);
        }
    }
    
    [ServerRpc]
    public void AddModuleRPC(HexCoordinate coord, NetModuleID id, int rotation)
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
           AddModuleReference(coord + rotatedLocalCoord, placementData);
        }
    }

    private IEnumerable<HexCoordinate> GetRotatedCoordinates(IEnumerable<HexCoordinate> coords, int rotationCount)
    {
        foreach (HexCoordinate coord in coords)
        {
            HexCoordinate rotatedCoord = coord;
            for (int i = 0; i < rotationCount; i++)
            {
                rotatedCoord = coord.RotateClockwise();
            }
            yield return rotatedCoord;
        }
    }

    private void AddModuleReference(HexCoordinate coord, ModulePlacementData data)
    {
        _moduleMap[coord] = data;
    }

    public void RemoveModule(HexCoordinate coord)
    {
        if (IsOwner)
        {
            RemoveModuleRPC(coord);
        }
    }

    [ServerRpc]
    public void RemoveModuleRPC(HexCoordinate coord)
    {
        ModulePlacementData placementData = _moduleMap[coord];
        var localHexCoordinates = placementData.ModuleID.GetModuleData().GetLocalHexCoordinates();
        foreach (var rotatedLocalCoord in GetRotatedCoordinates(localHexCoordinates, placementData.Rotation))
        {
            RemoveModuleReference(placementData.RootCoordinate + rotatedLocalCoord);
        }
    }

    private void RemoveModuleReference(HexCoordinate coord)
    {
        _moduleMap.Remove(coord);
    }

    public bool IsCoordinateOccupied(HexCoordinate coord)
    {
        return _moduleMap.ContainsKey(coord);
    }

    public bool IsNeighboringModule(HexCoordinate coord)
    {
        // The return line is the equivalent of the following code
        // foreach (HexCoordinate neighbor in coord.Neighbors()) 
        // {
        //      if (IsCoordinateOccupied(neighbor)) return true;
        // }
        // return false;
        return coord.Neighbors().Any(IsCoordinateOccupied);
    }

    // (HexCoordinate coord, NetModuleID) is a "Value Tuple" which lets one return multiple variables from a function
    public IEnumerable<(HexCoordinate coord, NetModuleID id)> GetNeighboringModules(HexCoordinate coord)
    {
        foreach (HexCoordinate neighbor in coord.Neighbors())
        {
            if (_moduleMap.TryGetValue(neighbor, out ModulePlacementData placementData))
            {
                yield return (coord, placementData.ModuleID);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (HexCoordinate coord in _moduleMap.Keys)
        {
            coord.DrawGizmos(Color.yellow, 2f, 0.9f);
        }
    }
}
