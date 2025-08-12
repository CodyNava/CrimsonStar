using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[System.Serializable]
public class NetLobbyData
{
    public ulong playerID;
    public string playerDisplayName;
    public NetTeamID playerTeamID;
    public bool isReady;
}

public class NetLobbyConductor : BaseConductor<NetLobbyConductor>
{
    [SerializeField] private NetMatchPlayer matchPrefab;
    [SerializeField] private NetShipEditorConductor netShipEditorConductor;
    [SerializeField] private NetGameplayConductor netGameplayConductor;
    [SerializeField]
    private SerializedDictionary<NetTeamModeID, string> _teamModeMaps = new();

    public string TeamModeMapName => _teamModeMaps[_selectedTeamMode];

    public NetworkObject[] Players { get; private set; }
    public Dictionary<ulong, NetMatchPlayer> PlayersByID { get; private set; }
    private readonly SyncDictionary<NetworkConnection, NetMatchPlayer> _playersByConnection = new();
    public SyncDictionary<NetworkConnection, NetMatchPlayer> PlayersByConnection => _playersByConnection;
    private readonly SyncDictionary<ulong, NetworkConnection> _connectionsByPlayerID = new();
    public SyncDictionary<ulong, NetworkConnection> ConnectionsByPlayerID => _connectionsByPlayerID;
    public Dictionary<NetworkConnection, NetLobbyData> ConnectionPlayerMap { get; } = new();

    private NetworkConnection _hostConnection;
    private NetGameModeID _selectedGameMode = NetGameModeID.DefaultMode;
    private NetTeamModeID _selectedTeamMode;
    public NetTeamModeID SelectedTeamMode => _selectedTeamMode;

    private float _updateAccumulator;

    // LobbySettings
    private readonly SyncVar<NetRefundModuleID> _refundModuleSetting = new();
    private readonly SyncVar<NetFirendlyFireID> _friendlyFireSetting = new();
    private readonly SyncVar<int> _roundCount = new();
    private readonly SyncVar<float> _editorTimerDuration = new();
    private readonly SyncVar<string> _gameplaySceneName = new();


    // Settings Accessors
    public NetFirendlyFireID FriendlyFireID
    {
        get => _friendlyFireSetting.Value;
        set
        {
            if (IsServerInitialized) _friendlyFireSetting.Value = value;
        }
    }
    
    public float FriendlyFireDamageMult
    {
        get
        {
            return FriendlyFireID switch
            {
                NetFirendlyFireID.Half => 0.5f,
                NetFirendlyFireID.Quarter => 0.25f,
                NetFirendlyFireID.Off => 0f,
                _ => 1f
            };
        }
    }
    
    
    public NetRefundModuleID RefundModuleID
    {
        get => _refundModuleSetting.Value;
        set
        {
            if (IsServerInitialized) _refundModuleSetting.Value = value;
        }
    }

    public float RefundModule
    {
        get
        {
            return RefundModuleID switch
            {
                NetRefundModuleID.Half => 0.5f,
                NetRefundModuleID.Quarter => 0.25f,
                NetRefundModuleID.Off => 0f,
                _ => 1f
            };
        }
    }

    public int RoundCount
    {
        get => _roundCount.Value;
        set
        {
            if (IsServerInitialized) _roundCount.Value = value;
        }
    }

    public float EditorTimerDuration
    {
        get => _editorTimerDuration.Value;
        set
        {
            if (IsServerInitialized) _editorTimerDuration.Value = value;
        }
    }
    
    public string GameplaySceneName
    {
        get => _gameplaySceneName.Value;
        set
        {
            if (IsServerInitialized) _gameplaySceneName.Value = value;
        }
    }


    public override string ConductedSceneName => "NetLobby";

    protected override void OnNetworkStarted()
    {
        _gameplaySceneName.Value = "NetGameplayScene";
        
        var shipEditorConductor = Instantiate(netShipEditorConductor);
        var gameplayConductor = Instantiate(netGameplayConductor);
        ServerManager.Spawn(shipEditorConductor.gameObject);
        ServerManager.Spawn(gameplayConductor.gameObject);

        ServerManager.OnRemoteConnectionState += S_OnConnectionStateChange;
        ServerManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerIdentified>(S_OnPlayerIdentified, false);
        ServerManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerTeamChangeRequested>(S_OnPlayerTeamChangeRequested,
            false);
        ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetGameMode>(S_OnGameModeChangeRequested, false);
        ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetTeamMode>(S_OnTeamModeChangeRequested, false);
        ServerManager.RegisterBroadcast<NetLobbyBroadcasts.SetReadyState>(S_OnPlayerReadyStateChanged, false);
        ServerManager.RegisterBroadcast<NetLobbyBroadcasts.GameStartRequested>(S_OnGameStartRequested, false);
    }

    private void Update()
    {
        if (!IsServerStarted) return;
        _updateAccumulator += Time.deltaTime;
        if (_updateAccumulator > 0.5f)
        {
            S_SendLobbySettingsUpdate();
            S_SendPlayerDataUpdate();
            _updateAccumulator -= 0.5f;
        }
    }

    private void S_OnTeamModeChangeRequested(NetworkConnection conn, NetLobbyBroadcasts.SetTeamMode msg,
        Channel channel)
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
        InstanceFinder.GetInstance<NetGameplayConductor>().SetGameplayScene(TeamModeMapName);
    }

    [Server]
    private void S_SetTeamsFreeForAll()
    {
        NetTeamID teamID = NetTeamID.Team1;
        foreach (var playerData in ConnectionPlayerMap.Values)
        {
            if (playerData.playerTeamID == NetTeamID.Observer) continue;
            playerData.playerTeamID = teamID;
            teamID++;
        }
    }

    [Server]
    private void S_OnPlayerReadyStateChanged(NetworkConnection conn, NetLobbyBroadcasts.SetReadyState msg,
        Channel channel)
    {
        if (!ConnectionPlayerMap.TryGetValue(conn, out var playerData))
            return;
        playerData.isReady = msg.ReadyState;
    }

    [Server]
    private void S_OnGameModeChangeRequested(NetworkConnection conn, NetLobbyBroadcasts.SetGameMode msg,
        Channel channel)
    {
        if (conn != _hostConnection) return;
        S_SetGameMode(msg.GameMode);
    }

    [Server]
    public void S_SetGameMode(NetGameModeID gameModeID)
    {
        _selectedGameMode = gameModeID;
    }

    [Server]
    private void S_OnPlayerTeamChangeRequested(NetworkConnection conn, NetLobbyBroadcasts.PlayerTeamChangeRequested msg,
        Channel channel)
    {
        foreach (var (connection, data) in ConnectionPlayerMap)
        {
            if (data.playerID != msg.PlayerID) continue;
            if (conn != connection && conn != _hostConnection) return;
            data.playerTeamID = msg.NewTeamID;
        }
    }

    [Server]
    private void S_OnPlayerIdentified(NetworkConnection conn, NetLobbyBroadcasts.PlayerIdentified msg, Channel channel)
    {
        AddPlayer(conn, msg);
    }

    [Server]
    public void AddPlayer(NetworkConnection conn, NetLobbyBroadcasts.PlayerIdentified msg)
    {
        ConnectionPlayerMap[conn] = new NetLobbyData
        {
            playerID = msg.PlayerID,
            playerDisplayName = msg.DisplayName,
            playerTeamID = NetTeamID.Team1
        };

        if (_selectedTeamMode == NetTeamModeID.FreeForAll)
        {
            S_SetTeamsFreeForAll();
        }

        if (msg.IsHost)
        {
            _hostConnection = conn;
            ConnectionPlayerMap[conn].isReady = true;
        }
    }

    [Server]
    public void S_SyncPreview(NetLobbyBroadcasts.PreviewUIElements preview)
    {
            ServerManager.Broadcast(preview, false);
    }

    [Server]
    private void S_OnConnectionStateChange(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            ConnectionPlayerMap[connection] = new NetLobbyData
            {
                playerDisplayName = "Connecting..."
            };
        }

        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            ConnectionPlayerMap.Remove(connection);
        }
    }

    [Server]
    private void S_SendPlayerDataUpdate()
    {
        ServerManager.Broadcast(new NetLobbyBroadcasts.PlayerListUpdate
        {
            Players = ConnectionPlayerMap.Values.ToArray(),
            TeamMode = _selectedTeamMode
        }, false);
    }

    [Server]
    private void S_SendLobbySettingsUpdate()
    {
        ServerManager.Broadcast(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = _selectedGameMode,
            BaseCurrency = DataProvider.GetStartingCurrency(_selectedGameMode),
            CurrencyAddedPerRound = DataProvider.GetCurrencyAddedPerRound(_selectedGameMode)
        }, false);
        ServerManager.Broadcast(new NetLobbyBroadcasts.SetTeamMode
        {
            TeamMode = _selectedTeamMode
        }, false);
    }

    [Server]
    private void S_OnGameStartRequested(NetworkConnection connection, NetLobbyBroadcasts.GameStartRequested msg,
        Channel channel)
    {
        if (!ConnectionPlayerMap.Values.All(player => player.isReady)) return;
        PrepareGame();
        InstanceFinder.GetInstance<NetShipEditorConductor>().MoveToScene(this, Players);
    }

    [Server]
    public void PrepareGame()
    {
        S_SetUpMatchPlayers();
        SceneAudioManager.instance.StopMainMusic();
        SceneAudioManager.instance.StartInGameMusic();
        C_TriggerSwapMusic();
    }


    [ObserversRpc]
    [Client]
    private void C_TriggerSwapMusic()
    {
        SwapMusic();
    }

    private void SwapMusic()
    {
        SceneAudioManager.instance.StopMainMusic();
        SceneAudioManager.instance.StartInGameMusic();
    }

    [Server]
    private void S_SetUpMatchPlayers()
    {
        if (Players != null)
        {
            foreach (NetworkObject player in Players)
            {
                if (player != null)
                {
                    ServerManager.Despawn(player.gameObject);
                }
            }
        }

        Players = new NetworkObject[ConnectionPlayerMap.Count];
        PlayersByID = new Dictionary<ulong, NetMatchPlayer>();
        _playersByConnection.Clear();
        _connectionsByPlayerID.Clear();
        int count = 0;
        foreach (var (conn, lobbyData) in ConnectionPlayerMap)
        {
            var player = Instantiate(matchPrefab);
            ServerManager.Spawn(player.gameObject, conn);
            player.S_Init(lobbyData, _selectedGameMode);
            Players[count++] = player.NetworkObject;
            PlayersByID.Add(player.PlayerID.Value, player);
            PlayersByConnection.Add(conn, player);
            ConnectionsByPlayerID.Add(player.PlayerID.Value, conn);
        }
    }

    protected override void OnNetworkStopped()
    {
        ServerManager.OnRemoteConnectionState -= S_OnConnectionStateChange;
        ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.PlayerIdentified>(S_OnPlayerIdentified);
        ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.GameStartRequested>(S_OnGameStartRequested);
        ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.PlayerTeamChangeRequested>(S_OnPlayerTeamChangeRequested);
        ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetGameMode>(S_OnGameModeChangeRequested);
        ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetTeamMode>(S_OnTeamModeChangeRequested);
        ServerManager.UnregisterBroadcast<NetLobbyBroadcasts.SetReadyState>(S_OnPlayerReadyStateChanged);
    }

   [Server]
    public int S_GetRoundCount() => _roundCount.Value;

    [Server]
    public int S_GetResourcePerRound() =>
        DataProvider.Instance.GameModeConfig.GetCurrencyAddedPerRound(_selectedGameMode);

    [Server]
    public int S_GetInitialResourceCount() => DataProvider.Instance.GameModeConfig.GetBaseCurrency(_selectedGameMode);
}