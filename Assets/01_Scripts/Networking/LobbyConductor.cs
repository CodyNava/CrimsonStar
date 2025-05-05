using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

[System.Serializable]
public struct LobbyPlayerData
{
    public CSteamID playerSteamID;
    public string playerDisplayName;
}

public class LobbyConductor : NetworkSingleton<LobbyConductor>
{
    private readonly Dictionary<NetworkConnection, LobbyPlayerData> _connectionPlayerMap = new();

    public override void OnStartNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.RegisterInstance(this);
            ServerManager.OnRemoteConnectionState += OnConnectionStateChange;
            ServerManager.RegisterBroadcast<LobbyBroadcasts.PlayerIdentified>(OnPlayerIdentified, false);
            ServerManager.RegisterBroadcast<LobbyBroadcasts.GameStartRequested>(OnGameStartRequested, false);
        }
    }

    private void OnPlayerIdentified(NetworkConnection conn, LobbyBroadcasts.PlayerIdentified msg, Channel channel)
    {
        _connectionPlayerMap[conn] = new LobbyPlayerData
        {
            playerSteamID = msg.SteamID,
            playerDisplayName = msg.DisplayName
        };
        SendPlayerDataUpdate();
    }

    private void OnConnectionStateChange(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            _connectionPlayerMap[connection] = new LobbyPlayerData
            {
                playerDisplayName = "Connecting..."
            };
        }

        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            _connectionPlayerMap.Remove(connection);
        }

        SendPlayerDataUpdate();
    }

    private void SendPlayerDataUpdate()
    {
        ServerManager.Broadcast(new LobbyBroadcasts.PlayerListUpdate
        {
            Players = _connectionPlayerMap.Values.ToArray()
        }, false);
    }

    private void OnGameStartRequested(NetworkConnection connection, LobbyBroadcasts.GameStartRequested msg, Channel channel)
    {
        Debug.Log("Game start requested");
        CloseLobbyScene();

        SceneLoadData sceneData = new("NetShipEditor");
        var connections = ServerManager.Clients.Values.ToArray();
        SceneManager.LoadConnectionScenes(connections, sceneData);
    }

    [ObserversRpc]
    private void CloseLobbyScene()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("NetLobby");
    }

    public override void OnStopNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.UnregisterInstance<LobbyConductor>();
            ServerManager.OnRemoteConnectionState -= OnConnectionStateChange;
            ServerManager.UnregisterBroadcast<LobbyBroadcasts.PlayerIdentified>(OnPlayerIdentified);
            ServerManager.UnregisterBroadcast<LobbyBroadcasts.GameStartRequested>(OnGameStartRequested);
        }
    }
}
