using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class NetShipEditorData : NetworkBehaviour
{
    public NetModuleStorage ModuleStorage { get; private set; }
    public NetResourceStorage ResourceStorage { get; private set; }

    public void SetResourceCounts(Dictionary<NetCurrencyType, int> resources)
    {
        ModuleStorage ??= GetComponent<NetModuleStorage>();
        ResourceStorage ??= GetComponent<NetResourceStorage>();
        ResourceStorage.SetResourceCounts(resources);
    }
    
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        if (IsOwner)
        {
            ModuleStorage ??= GetComponent<NetModuleStorage>();
            ResourceStorage ??= GetComponent<NetResourceStorage>();
            StartCoroutine(LinkToEditor());
        }
    }

    private IEnumerator LinkToEditor()
    {
        ShipEditor shipEditor = null;
        while (!shipEditor)
        {
            shipEditor = FindFirstObjectByType<ShipEditor>();
            yield return null;
        }
        
        shipEditor.SetPlayerShipEditor(this);
    }

    public bool SignalReady()
    {
        var conductor = InstanceFinder.GetInstance<NetShipEditorConductor>();
        if (conductor != null)
        {
            conductor.SignalReady();
            return true;
        }

        Debug.LogError("Couldn't find Ship Editor Conductor!");
        return false;
    }
}
