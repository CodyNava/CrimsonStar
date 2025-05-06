using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;

public class PlayerShipEditor : NetworkBehaviour
{
    public ServerModuleStorage ModuleStorage { get; private set; }
    public ServerResourceStorage ResourceStorage { get; private set; }

    public void Initialize(Dictionary<NetResourceType, int> resources)
    {
        ModuleStorage = GetComponent<ServerModuleStorage>();
        ResourceStorage = GetComponent<ServerResourceStorage>();
        ResourceStorage.SetResourceCounts(resources);
    }
    
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        StartCoroutine(LinkToEditor());
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
}
