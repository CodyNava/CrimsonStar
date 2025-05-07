using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class ServerResourceStorage : NetworkBehaviour
{
    private readonly SyncDictionary<NetResourceType, int> _resourceStorage = new();

    public void SetResourceCounts(Dictionary<NetResourceType, int> resourceCounts)
    {
        _resourceStorage.Clear();
        foreach ((NetResourceType resource, int count) in resourceCounts)
        {
            _resourceStorage[resource] = count;
        }
    }
    
    public bool HasResourcesForModule(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetResourceType resourceType, int cost) in moduleData.Costs)
        {
            if (!_resourceStorage.TryGetValue(resourceType, out int storage)) return false;
            if (storage < cost) return false;
        }

        return true;
    }

    [ServerRpc]
    public void PayForModule(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetResourceType resourceType, int cost) in moduleData.Costs)
        {
            _resourceStorage[resourceType] -= cost;
        }
    }

    [ServerRpc]
    public void RefundModule(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetResourceType resourceType, int cost) in moduleData.Costs)
        {
            _resourceStorage[resourceType] += cost;
        }
    }

    public int GetRemainingResourceCount(NetResourceType type)
    {
        return _resourceStorage.GetValueOrDefault(type, 0);
    }
}
