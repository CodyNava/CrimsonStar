using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class ServerShipEditorData : NetworkBehaviour
{
    public ServerModuleStorage ModuleStorage { get; private set; }
    public ServerResourceStorage ResourceStorage { get; private set; }

    public void SetResourceCounts(Dictionary<NetResourceType, int> resources)
    {
        ModuleStorage ??= GetComponent<ServerModuleStorage>();
        ResourceStorage ??= GetComponent<ServerResourceStorage>();
        ResourceStorage.SetResourceCounts(resources);
    }
    
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        if (IsOwner)
        {
            ModuleStorage ??= GetComponent<ServerModuleStorage>();
            ResourceStorage ??= GetComponent<ServerResourceStorage>();
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
        var conductor = InstanceFinder.GetInstance<ShipEditorConductor>();
        if (conductor != null)
        {
            conductor.SignalReady();
            return true;
        }

        Debug.LogError("Couldn't find Ship Editor Conductor!");
        return false;
    }
}
