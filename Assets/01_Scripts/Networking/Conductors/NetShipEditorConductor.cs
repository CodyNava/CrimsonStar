using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Serialization;

public class NetShipEditorConductor : NetworkSingleton<NetShipEditorConductor>
{
    [SerializeField] private NetGameplayConductor netGameplayConductor;
    [SerializeField] private NetShipEditorData shipEditorDataPrefab;
    private Dictionary<NetworkConnection, bool> _playersReady = new();

    public Dictionary<NetworkConnection, NetShipEditorData> PlayerShipEditors { get; } = new();

    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            SceneManager.OnClientPresenceChangeStart += OnSceneChange;
        }
    }

    public override void OnStopNetwork()
    {
        InstanceFinder.UnregisterInstance<NetShipEditorConductor>();
        
        if (IsServerInitialized)
        {
            SceneManager.OnClientPresenceChangeStart -= OnSceneChange;
        }
    }

    private void OnSceneChange(ClientPresenceChangeEventArgs args)
    {
        var defaultResources = DataProvider.Instance.DefaultEditorResources;
        
        if (args.Scene.name == "NetShipEditor" && args.Added)
        {
            var shipEditor = Instantiate(shipEditorDataPrefab);
            shipEditor.SetResourceCounts(defaultResources.DefaultResourceCounts);
            ServerManager.Spawn(shipEditor.gameObject, args.Connection, args.Scene);
            PlayerShipEditors.Add(args.Connection, shipEditor);
            _playersReady.Add(args.Connection, false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SignalReady(Channel channel = Channel.Reliable, NetworkConnection conn = null)
    {
        _playersReady[conn!] = true;

        if (AllPlayersReady())
        {
            Debug.Log("Game start requested");

            SceneLoadData sceneData = new("NetGameplayScene");
            sceneData.PreferredActiveScene = new PreferredScene(sceneData.SceneLookupDatas[0]);
            SceneUnloadData unloadData = new("NetShipEditor");
            SceneManager.LoadGlobalScenes(sceneData);
            SceneManager.UnloadGlobalScenes(unloadData);

            GameObject gameConductor = Instantiate(netGameplayConductor).gameObject;
            ServerManager.Spawn(gameConductor);
        }
    }

    private bool AllPlayersReady()
    {
        return _playersReady.Values.All(ready => ready);
    }
}
