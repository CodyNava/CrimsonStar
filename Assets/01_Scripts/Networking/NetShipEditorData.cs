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

    public void S_SetResourceCount(int count)
    {
        ModuleStorage ??= GetComponent<NetModuleStorage>();
        ResourceStorage ??= GetComponent<NetResourceStorage>();
        ResourceStorage.S_SetResourceCount(count);
    }
    
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        if (IsOwner)
        {
            ModuleStorage ??= GetComponent<NetModuleStorage>();
            ResourceStorage ??= GetComponent<NetResourceStorage>();
            StartCoroutine(LinkToEditor(true));
        }
    }

    [ObserversRpc]
    public void Relink()
    {
        if (IsOwner)
        {
            StartCoroutine(LinkToEditor(false));
        }
    }

    private IEnumerator LinkToEditor(bool isInit)
    {
        ShipEditor shipEditor = null;
        while (!shipEditor)
        {
            shipEditor = FindFirstObjectByType<ShipEditor>();
            yield return null;
        }
        
        shipEditor.SetPlayerShipEditor(this);
        if (!isInit)
        {
            shipEditor.ReconstructShip(ModuleStorage.GetUniqueModules());
        }
    }

    public bool SignalReady()
    {
        var conductor = InstanceFinder.GetInstance<NetShipEditorConductor>();
        if (conductor != null)
        {
            conductor.S_SignalReady();
            return true;
        }

        Debug.LogError("Couldn't find Ship Editor Conductor!");
        return false;
    }
}
