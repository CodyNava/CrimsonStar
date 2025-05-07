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

public class ServerModuleStorage : NetworkBehaviour
{
    private static readonly HexCoordinate[] BridgeCoordinates = {
        HexCoordinate.Zero,
        HexCoordinate.Neighbor(HexCoordinate.Zero, HexDirection.North),
    };

    public SyncDictionary<HexCoordinate, ModulePlacementData> ModuleMap { get; } = new();

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
            ModuleMap[bridgeCoord] = bridgeData;
        }
    }

    public void AddModule(HexCoordinate coord, NetModuleID id, int rotation)
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
        ModuleMap[coord] = data;
    }

    public void RemoveModule(HexCoordinate coord)
    {
        ModulePlacementData placementData = ModuleMap[coord];
        var localHexCoordinates = placementData.ModuleID.GetModuleData().GetLocalHexCoordinates();
        foreach (var rotatedLocalCoord in GetRotatedCoordinates(localHexCoordinates, placementData.Rotation))
        {
            RemoveModuleReference(placementData.RootCoordinate + rotatedLocalCoord);
        }
    }

    private void RemoveModuleReference(HexCoordinate coord)
    {
        ModuleMap.Remove(coord);
    }

    public bool IsCoordinateOccupied(HexCoordinate coord)
    {
        return ModuleMap.ContainsKey(coord);
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
            if (ModuleMap.TryGetValue(neighbor, out ModulePlacementData placementData))
            {
                yield return (coord, placementData.ModuleID);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (HexCoordinate coord in ModuleMap.Keys)
        {
            coord.DrawGizmos(Color.yellow, 2f, 0.9f);
        }
    }
}
