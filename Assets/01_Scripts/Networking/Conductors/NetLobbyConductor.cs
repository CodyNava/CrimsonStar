using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct LobbyPlayerData
{
    public CSteamID playerSteamID;
    public string playerDisplayName;
}

public class NetLobbyConductor : NetworkSingleton<NetLobbyConductor>
{
    [SerializeField] private NetShipEditorConductor netShipEditorConductor;
    
    private readonly Dictionary<NetworkConnection, LobbyPlayerData> _connectionPlayerMap = new();

    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            ServerManager.OnRemoteConnectionState += S_OnConnectionStateChange;
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerIdentified>(S_OnPlayerIdentified, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.GameStartRequested>(S_OnGameStartRequested, false);
        }
    }

    private void S_OnPlayerIdentified(NetworkConnection conn, NetLobbyBroadcasts.PlayerIdentified msg, Channel channel)
    {
        _connectionPlayerMap[conn] = new LobbyPlayerData
        {
            playerSteamID = msg.SteamID,
            playerDisplayName = msg.DisplayName
        };
        S_SendPlayerDataUpdate();
    }

    private void S_OnConnectionStateChange(NetworkConnection connection, RemoteConnectionStateArgs args)
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

        S_SendPlayerDataUpdate();
    }

    private void S_SendPlayerDataUpdate()
    {
        ServerManager.Broadcast(new NetLobbyBroadcasts.PlayerListUpdate
        {
            Players = _connectionPlayerMap.Values.ToArray()
        }, false);
    }

    private void S_OnGameStartRequested(NetworkConnection connection, NetLobbyBroadcasts.GameStartRequested msg, Channel channel)
    {
        SceneLoadData sceneData = new("NetShipEditor");
        sceneData.PreferredActiveScene = new PreferredScene(sceneData.SceneLookupDatas[0]);
        SceneUnloadData unloadData = new("NetLobby");
        SceneManager.LoadGlobalScenes(sceneData);
        SceneManager.UnloadGlobalScenes(unloadData);

        GameObject shipEditor = Instantiate(netShipEditorConductor).gameObject;
        ServerManager.Spawn(shipEditor);
    }

    public override void OnStopNetwork()
    {
        InstanceFinder.UnregisterInstance<NetLobbyConductor>();
        
        if (IsServerInitialized)
        {
            ServerManager.OnRemoteConnectionState -= S_OnConnectionStateChange;
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.PlayerIdentified>(S_OnPlayerIdentified);
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.GameStartRequested>(S_OnGameStartRequested);
        }
    }
}
