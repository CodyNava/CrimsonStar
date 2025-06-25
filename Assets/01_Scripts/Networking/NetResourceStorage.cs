using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class NetResourceStorage : NetworkBehaviour
{
    private readonly SyncDictionary<NetCurrencyType, int> _resourceStorage = new();

    public void S_SetResourceCount(int count)
    {
        _resourceStorage.Clear();
        _resourceStorage[NetCurrencyType.Gold] = count;
    }
    
    public bool SC_HasResourcesForModule(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetCurrencyType resourceType, int cost) in moduleData.Costs)
        {
            if (!_resourceStorage.TryGetValue(resourceType, out int storage)) return false;
            if (storage < cost) return false;
        }

        return true;
    }

    public void C_PayForModule(NetModuleID id)
    {
        if (IsOwner)
        {
            S_PayForModuleRPC(id);
        }
    }
    
    [ServerRpc]
    public void S_PayForModuleRPC(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetCurrencyType resourceType, int cost) in moduleData.Costs)
        {
            int amount = _resourceStorage.GetValueOrDefault(resourceType) - cost;
            _resourceStorage[resourceType] = amount;
        }
    }

    public void C_RefundModule(NetModuleID id)
    {
        if (IsOwner)
        {
            S_RefundModuleRPC(id);
        }
    }
    
    [ServerRpc]
    public void S_RefundModuleRPC(NetModuleID moduleID)
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];
        foreach ((NetCurrencyType resourceType, int cost) in moduleData.Costs)
        {
            int amount = _resourceStorage.GetValueOrDefault(resourceType) + cost;
            _resourceStorage[resourceType] = amount;
        }
    }

    public int C_GetRemainingResourceCount(NetCurrencyType type)
    {
        return _resourceStorage.GetValueOrDefault(type, 0);
    }

    public void S_AddResourceCount(NetCurrencyType type, int addedAmount)
    {
        int amount = _resourceStorage.GetValueOrDefault(type) + addedAmount;
        _resourceStorage[type] = amount;
    }
}
