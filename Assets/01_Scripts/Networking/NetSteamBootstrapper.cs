using System;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;
using Fishy = FishySteamworks.FishySteamworks;

public class NetSteamBootstrapper : SceneSingleton<NetSteamBootstrapper>
{
    [Header("Dependencies")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Fishy steamTransport;
    [Header("Conductors")]
    [SerializeField] private LobbyConductor lobbyConductor;
    
    protected Callback<LobbyCreated_t> SteamLobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> SteamLobbyJoinRequested;
    protected Callback<LobbyEnter_t> SteamLobbyEnter;

    private void OnEnable()
    {
        steamTransport.OnClientConnectionState += OnConnectionState;
    }

    private void OnDisable()
    {
        steamTransport.OnClientConnectionState -= OnConnectionState;
    }

    private void OnConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            InstanceFinder.ClientManager.Broadcast(new LobbyBroadcasts.PlayerJoined
            {
                SteamID = SteamPlayer.SteamID,
                DisplayName = SteamPlayer.DisplayName
            });
        }
    }

    private void Start()
    {
        SteamLobbyCreated = Callback<LobbyCreated_t>.Create(OnSteamLobbyCreated);
        SteamLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnSteamLobbyJoinRequested);
        SteamLobbyEnter = Callback<LobbyEnter_t>.Create(OnSteamLobbyEnter);
        
        SteamPlayer.SetUserID(SteamUser.GetSteamID());
        SteamPlayer.SetDisplayName(SteamFriends.GetPersonaName());
    }

    // Code-path for initializing the server (host only)
    private void OnSteamLobbyCreated(LobbyCreated_t data)
    {
        if (data.m_eResult != EResult.k_EResultOK)
            return;

        string host = SteamPlayer.SteamID.ToString();
        
        SteamPlayer.SetLobbyID(data.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(SteamPlayer.CurrentLobbyID, SteamLobby.HostKey, host);
        steamTransport.SetClientAddress(host);
        steamTransport.StartConnection(true); // This starts only server on host
        var lobbyConductorGo = Instantiate(lobbyConductor).gameObject;
        InstanceFinder.ServerManager.Spawn(lobbyConductorGo);
        SteamPlayer.SetLobbyHost(true);
    }

    private void OnSteamLobbyJoinRequested(GameLobbyJoinRequested_t data)
    {
        SteamMatchmaking.JoinLobby(data.m_steamIDLobby);
    }

    // Code-path for initializing the client (host and other clients)
    private void OnSteamLobbyEnter(LobbyEnter_t data)
    {
        SteamPlayer.SetLobbyID(data.m_ulSteamIDLobby);

        string host = SteamMatchmaking.GetLobbyData(SteamPlayer.CurrentLobbyID, SteamLobby.HostKey);
        steamTransport.SetClientAddress(host);
        steamTransport.StartConnection(false);
    }

    public static void LeaveLobby()
    {
        InstanceFinder.ClientManager.Broadcast(new LobbyBroadcasts.PlayerLeft()
        {
            SteamID = SteamPlayer.SteamID
        });
        
        SteamMatchmaking.LeaveLobby(SteamPlayer.CurrentLobbyID);
        SteamPlayer.SetLobbyID(0);

        Instance.steamTransport.StopConnection(false);
        if (Instance.networkManager.IsServerStarted)
            Instance.steamTransport.StopConnection(true);
    }
}
