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
    [SerializeField] private PlayerShipEditor shipEditorPrefab;
    private Dictionary<NetworkConnection, PlayerShipEditor> _playerShipEditors = new();
    private Dictionary<NetworkConnection, bool> _playersReady = new();

    public override void OnStartNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.RegisterInstance(this);
            SceneManager.OnClientPresenceChangeStart += OnSceneChange;
        }
    }

    public override void OnStopNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.UnregisterInstance<ShipEditorConductor>();
            SceneManager.OnClientPresenceChangeStart -= OnSceneChange;
        }
    }

    private void OnSceneChange(ClientPresenceChangeEventArgs args)
    {
        var defaultResources = DataProvider.Instance.DefaultEditorResources;
        
        if (args.Scene.name == "NetShipEditor" && args.Added)
        {
            var shipEditor = Instantiate(shipEditorPrefab);
            shipEditor.Initialize(defaultResources.DefaultResourceCounts);
            ServerManager.Spawn(shipEditor.gameObject, args.Connection, args.Scene);
            _playerShipEditors.Add(args.Connection, shipEditor);
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
            CloseEditorScene();

            SceneLoadData sceneData = new("NetGameplayScene");
            var connections = ServerManager.Clients.Values.ToArray();
            SceneManager.LoadConnectionScenes(connections, sceneData);
        }
    }

    [ObserversRpc]
    private void CloseEditorScene()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("NetShipEditor");
    }

    private bool AllPlayersReady()
    {
        return _playersReady.Values.All(ready => ready);
    }
}
