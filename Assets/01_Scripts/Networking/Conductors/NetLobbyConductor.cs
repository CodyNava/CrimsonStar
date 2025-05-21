using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

[System.Serializable]
public class NetPlayerData
{
    public CSteamID playerSteamID;
    public string playerDisplayName;
    public NetTeamID playerTeamID;
    public bool isReady;
}

public class NetLobbyConductor : NetworkSingleton<NetLobbyConductor>
{
    [SerializeField] private NetShipEditorConductor netShipEditorConductor;

    public Dictionary<NetworkConnection, NetPlayerData> ConnectionPlayerMap { get; } = new();

    private NetworkConnection _hostConnection;
    private NetGameModeID _selectedGameMode;
    private NetTeamModeID _selectedTeamMode;

    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            ServerManager.OnRemoteConnectionState += S_OnConnectionStateChange;
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerIdentified>(S_OnPlayerIdentified, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerTeamChangeRequested>(S_OnPlayerTeamChangeRequested, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetGameMode>(S_OnGameModeChangeRequested, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetTeamMode>(S_OnTeamModeChangeRequested, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetReadyState>(S_OnPlayerReadyStateChanged, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.GameStartRequested>(S_OnGameStartRequested, false);
        }
    }

    private void S_OnTeamModeChangeRequested(NetworkConnection conn, NetLobbyBroadcasts.SetTeamMode msg, Channel channel)
    {
        NetTeamID teamID = NetTeamID.Team1;
        foreach (var playerData in ConnectionPlayerMap.Values)
        {
            if (playerData.playerTeamID == NetTeamID.Observer) continue;

            if (msg.TeamMode == NetTeamModeID.FreeForAll)
            {
                playerData.playerTeamID = teamID;
                teamID++;
            }
            else
            {
                playerData.playerTeamID = NetTeamID.Team1;
            }
        }

        _selectedTeamMode = msg.TeamMode;
        
        S_SendPlayerDataUpdate();
    }

    private void S_OnPlayerReadyStateChanged(NetworkConnection conn, NetLobbyBroadcasts.SetReadyState msg, Channel channel)
    {
        if (!ConnectionPlayerMap.TryGetValue(conn, out var playerData))
            return;
        playerData.isReady = msg.ReadyState;
        S_SendPlayerDataUpdate();
    }

    private void S_OnGameModeChangeRequested(NetworkConnection conn, NetLobbyBroadcasts.SetGameMode msg, Channel channel)
    {
        if (conn != _hostConnection) return;
        _selectedGameMode = msg.GameMode;
        S_SendLobbySettingsUpdate();
    }

    private void S_OnPlayerTeamChangeRequested(NetworkConnection conn, NetLobbyBroadcasts.PlayerTeamChangeRequested msg, Channel channel)
    {
        foreach (var (connection, data) in ConnectionPlayerMap)
        {
            if (data.playerSteamID != msg.Player) continue;
            if (conn != connection && conn != _hostConnection) return;
            data.playerTeamID = msg.NewTeamID;
            S_SendPlayerDataUpdate();
        }
    }

    private void S_OnPlayerIdentified(NetworkConnection conn, NetLobbyBroadcasts.PlayerIdentified msg, Channel channel)
    {
        ConnectionPlayerMap[conn] = new NetPlayerData
        {
            playerSteamID = msg.SteamID,
            playerDisplayName = msg.DisplayName,
            playerTeamID = NetTeamID.Team1
        };
        if (msg.IsHost)
        {
            _hostConnection = conn;
            ConnectionPlayerMap[conn].isReady = true;
        }
        S_SendPlayerDataUpdate();
    }

    private void S_OnConnectionStateChange(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            ConnectionPlayerMap[connection] = new NetPlayerData
            {
                playerDisplayName = "Connecting..."
            };
        }

        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            ConnectionPlayerMap.Remove(connection);
        }

        S_SendPlayerDataUpdate();
    }

    private void S_SendPlayerDataUpdate()
    {
        ServerManager.Broadcast(new NetLobbyBroadcasts.PlayerListUpdate
        {
            Players = ConnectionPlayerMap.Values.ToArray(),
            TeamMode = _selectedTeamMode
        }, false);
    }

    private void S_SendLobbySettingsUpdate()
    {
        ServerManager.Broadcast(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = _selectedGameMode
        }, false);
    }

    private void S_OnGameStartRequested(NetworkConnection connection, NetLobbyBroadcasts.GameStartRequested msg, Channel channel)
    {
        if (!ConnectionPlayerMap.Values.All(player => player.isReady)) return;
        
        SceneLoadData sceneData = new("NetShipEditor");
        sceneData.PreferredActiveScene = new PreferredScene(sceneData.SceneLookupDatas[0]);
        SceneUnloadData unloadData = new("NetLobby");
        SceneManager.LoadGlobalScenes(sceneData);
        SceneManager.UnloadGlobalScenes(unloadData);

        if (!InstanceFinder.HasInstance<NetGameplayConductor>())
        {
            GameObject shipEditor = Instantiate(netShipEditorConductor).gameObject;
            ServerManager.Spawn(shipEditor);
        }
    }

    public override void OnStopNetwork()
    {
        InstanceFinder.UnregisterInstance<NetLobbyConductor>();
        
        if (IsServerInitialized)
        {
            ServerManager.OnRemoteConnectionState -= S_OnConnectionStateChange;
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.PlayerIdentified>(S_OnPlayerIdentified);
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.GameStartRequested>(S_OnGameStartRequested);
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.PlayerTeamChangeRequested>(S_OnPlayerTeamChangeRequested);
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetGameMode>(S_OnGameModeChangeRequested);
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetTeamMode>(S_OnTeamModeChangeRequested);
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetReadyState>(S_OnPlayerReadyStateChanged);
        }
    }

    public NetPlayerData S_GetPlayerData(NetworkConnection connection) => ConnectionPlayerMap[connection];
    
    public int S_GetRoundCount() => 3;
    public int S_GetResourcePerRound() => DataProvider.Instance.GameModeConfig.Descriptions[_selectedGameMode].CurrencyAddedPerRound;
    public int S_GetInitialResourceCount() => DataProvider.Instance.GameModeConfig.Descriptions[_selectedGameMode].BaseCurrency;
}
