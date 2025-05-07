using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class ShipEditorConductor : NetworkSingleton<ShipEditorConductor>
{
    [SerializeField] private GameplayConductor gameplayConductor;
    [SerializeField] private ServerShipEditorData shipEditorDataPrefab;
    private Dictionary<NetworkConnection, bool> _playersReady = new();

    public Dictionary<NetworkConnection, ServerShipEditorData> PlayerShipEditors { get; } = new();

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
        InstanceFinder.UnregisterInstance<ShipEditorConductor>();
        
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
            ServerManager.Spawn(shipEditor.gameObject, args.Connection, args.Scene);
            shipEditor.Initialize();
            shipEditor.SetResourceCounts(defaultResources.DefaultResourceCounts);
            InitializeShipEditor(args.Connection, shipEditor, defaultResources.DefaultResourceCounts);
            PlayerShipEditors.Add(args.Connection, shipEditor);
            _playersReady.Add(args.Connection, false);
        }
    }

    [TargetRpc]
    public void InitializeShipEditor(NetworkConnection conn, ServerShipEditorData shipEditor, Dictionary<NetResourceType, int> resourceCounts)
    {
        shipEditor.Initialize();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SignalReady(Channel channel = Channel.Reliable, NetworkConnection conn = null)
    {
        _playersReady[conn!] = true;

        if (AllPlayersReady())
        {
            Debug.Log("Game start requested");

            SceneLoadData sceneData = new("NetGameplayScene");
            var connections = ServerManager.Clients.Values.ToArray();
            SceneManager.LoadConnectionScenes(connections, sceneData);

            CloseEditorScene();
            GameObject gameConductor = Instantiate(gameplayConductor).gameObject;
            ServerManager.Spawn(gameConductor);
        }
    }

    [ObserversRpc]
    private void CloseEditorScene()
    {
        var editorModules = FindObjectsByType<NetEditorModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (NetEditorModule editorModule in editorModules)
        {
            Destroy(editorModule.gameObject);
        }
        
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("NetShipEditor");
    }

    private bool AllPlayersReady()
    {
        return _playersReady.Values.All(ready => ready);
    }
}
