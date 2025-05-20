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
    private int _initialResourceCount = 1000;
    private int _roundCount = 3;
    private int _resourcesAddedPerRound = 0;
    private float _moduleRecycleRate;

    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            ServerManager.OnRemoteConnectionState += S_OnConnectionStateChange;
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerIdentified>(S_OnPlayerIdentified, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerTeamChangeRequested>(S_OnPlayerTeamChangeRequested, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetLobbySettings>(S_OnLobbySettingsChangeRequested, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetReadyState>(S_OnPlayerReadyStateChanged, false);
            ServerManager.RegisterBroadcast<NetLobbyBroadcasts.GameStartRequested>(S_OnGameStartRequested, false);
        }
    }

    private void S_OnPlayerReadyStateChanged(NetworkConnection conn, NetLobbyBroadcasts.SetReadyState msg, Channel channel)
    {
        if (!ConnectionPlayerMap.TryGetValue(conn, out var playerData))
            return;
        playerData.isReady = msg.ReadyState;
        S_SendPlayerDataUpdate();
    }

    private void S_OnLobbySettingsChangeRequested(NetworkConnection conn, NetLobbyBroadcasts.SetLobbySettings msg, Channel channel)
    {
        if (conn != _hostConnection) return;
        _initialResourceCount = msg.InitialResourceCount;
        _roundCount = msg.NumberOfRounds;
        _resourcesAddedPerRound = msg.ResourceGainPerRound;
        _moduleRecycleRate = msg.ModuleRecycleRate;
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
            Players = ConnectionPlayerMap.Values.ToArray()
        }, false);
    }

    private void S_SendLobbySettingsUpdate()
    {
        ServerManager.Broadcast(new NetLobbyBroadcasts.SetLobbySettings
        {
            InitialResourceCount = _initialResourceCount,
            NumberOfRounds = _roundCount,
            ResourceGainPerRound = _resourcesAddedPerRound,
            ModuleRecycleRate = _moduleRecycleRate
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
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetLobbySettings>(S_OnLobbySettingsChangeRequested);
            ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetReadyState>(S_OnPlayerReadyStateChanged);
        }
    }

    public NetPlayerData S_GetPlayerData(NetworkConnection connection) => ConnectionPlayerMap[connection];
    
    public int S_GetRoundCount() => _roundCount;
    public int S_GetResourcePerRound() => _resourcesAddedPerRound;
    public int S_GetInitialResourceCount() => _initialResourceCount;
}
