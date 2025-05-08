using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetResourceStorage : NetworkBehaviour
{
    private readonly SyncDictionary<NetCurrencyType, int> _resourceStorage = new();

    public void SetResourceCounts(Dictionary<NetCurrencyType, int> resourceCounts)
    {
        _resourceStorage.Clear();
        foreach ((NetCurrencyType resource, int count) in resourceCounts)
        {
            _resourceStorage[resource] = count;
        }
    }
    
    public bool HasResourcesForModule(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetCurrencyType resourceType, int cost) in moduleData.Costs)
        {
            if (!_resourceStorage.TryGetValue(resourceType, out int storage)) return false;
            if (storage < cost) return false;
        }

        return true;
    }

    public void PayForModule(NetModuleID id)
    {
        if (IsOwner)
        {
            PayForModuleRPC(id);
        }
    }
    
    [ServerRpc]
    public void PayForModuleRPC(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetCurrencyType resourceType, int cost) in moduleData.Costs)
        {
            _resourceStorage[resourceType] -= cost;
        }
        Debug.Log($"Currency left: {_resourceStorage[NetCurrencyType.Gold]}");
    }

    public void RefundModule(NetModuleID id)
    {
        if (IsOwner)
        {
            RefundModuleRPC(id);
        }
    }
    
    [ServerRpc]
    public void RefundModuleRPC(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetCurrencyType resourceType, int cost) in moduleData.Costs)
        {
            _resourceStorage[resourceType] += cost;
        }
        Debug.Log($"Currency left: {_resourceStorage[NetCurrencyType.Gold]}");
    }

    public int GetRemainingResourceCount(NetCurrencyType type)
    {
        return _resourceStorage.GetValueOrDefault(type, 0);
    }
}
