using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class ServerModuleStorage : NetworkBehaviour
{
    private static readonly HexCoordinate[] BridgeCoordinates = {
        HexCoordinate.Zero, 
        HexCoordinate.Neighbor(HexCoordinate.Zero, HexDirection.North),
    };
    
    private readonly SyncDictionary<HexCoordinate, NetModuleID> _modules = new();

    private void Awake()
    {
        foreach (HexCoordinate coord in BridgeCoordinates)
        {
            _modules.Add(coord, NetModuleID.Bridge);
        }
    }

    public void AddModule(HexCoordinate coord, NetModuleID id)
    {
        _modules.Add(coord, id);
    }

    public void RemoveModule(HexCoordinate coord)
    {
        _modules.Remove(coord);
    }

    public bool IsCoordinateOccupied(HexCoordinate coord)
    {
        return _modules.ContainsKey(coord);
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
            if (_modules.TryGetValue(neighbor, out NetModuleID moduleID))
            {
                yield return (coord, moduleID);
            }
        }
    }
}
