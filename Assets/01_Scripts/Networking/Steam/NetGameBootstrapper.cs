using FishNet;
using FishNet.Managing;
using FishNet.Managing.Transporting;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fishy = FishySteamworks.FishySteamworks;

public class NetGameBootstrapper : SceneSingleton<NetGameBootstrapper>
{
    [Header("Dependencies")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Fishy steamTransport;
    [SerializeField] private string mainMenuSceneName;
    [Header("Conductors")]
    [SerializeField] private NetLobbyConductor netLobbyConductor;
    
    protected Callback<LobbyCreated_t> SteamLobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> SteamLobbyJoinRequested;
    protected Callback<LobbyEnter_t> SteamLobbyEnter;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Additive);
    }
    
    private void OnEnable()
    {
        InstanceFinder.TransportManager.Transport.OnClientConnectionState += OnConnectionState;
    }

    private void OnDisable()
    {
        InstanceFinder.TransportManager.Transport.OnClientConnectionState -= OnConnectionState;
    }

    private void OnConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.PlayerIdentified
            {
                PlayerID = PlayerData.PlayerID,
                DisplayName = PlayerData.DisplayName,
                IsHost = PlayerData.IsLobbyHost
            });
        }
    }

    private void Start()
    {
        SteamLobbyCreated = Callback<LobbyCreated_t>.Create(OnSteamLobbyCreated);
        SteamLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnSteamLobbyJoinRequested);
        SteamLobbyEnter = Callback<LobbyEnter_t>.Create(OnSteamLobbyEnter);
        
        PlayerData.SetPlayerIDFromSteam(SteamUser.GetSteamID());
        PlayerData.SetDisplayName(SteamFriends.GetPersonaName());
    }

    // Code-path for initializing the server (host only)
    private void OnSteamLobbyCreated(LobbyCreated_t data)
    {
        if (data.m_eResult != EResult.k_EResultOK)
            return;

        string host = PlayerData.PlayerID.ToString();
        
        PlayerData.SetLobbyID(data.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(PlayerData.CurrentLobbyID, SteamLobby.HostKey, host);
        steamTransport.SetClientAddress(host);
        steamTransport.StartConnection(true); // This starts only server on host
        var lobbyConductorGo = Instantiate(netLobbyConductor).gameObject;
        InstanceFinder.ServerManager.Spawn(lobbyConductorGo);
        PlayerData.SetLobbyHost(true);
    }

    private void OnSteamLobbyJoinRequested(GameLobbyJoinRequested_t data)
    {
        SteamMatchmaking.JoinLobby(data.m_steamIDLobby);
    }

    // Code-path for initializing the client (host and other clients)
    private void OnSteamLobbyEnter(LobbyEnter_t data)
    {
        PlayerData.SetLobbyID(data.m_ulSteamIDLobby);
        Multipass mp = InstanceFinder.TransportManager.GetTransport<Multipass>();
        mp.SetClientTransport<Fishy>();

        string host = SteamMatchmaking.GetLobbyData(PlayerData.CurrentLobbyID, SteamLobby.HostKey);
        steamTransport.SetClientAddress(host);
        steamTransport.StartConnection(false);
        SceneManager.LoadScene("NetLobby");
    }

    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 9);
    }

    public static void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(PlayerData.CurrentLobbyID);
        PlayerData.SetLobbyID(0);

        Instance.steamTransport.StopConnection(false);
        if (Instance.networkManager.IsServerStarted)
            Instance.steamTransport.StopConnection(true);
    }

    public static void LeaveLobbyLocal()
    {
        InstanceFinder.ClientManager.StopConnection();
        if (InstanceFinder.NetworkManager.IsServerStarted)
        {
            InstanceFinder.ServerManager.StopConnection(true);
        }
    }

    public static void CreateLobbyLocal()
    {
        PlayerData.SetPlayerIDFromRandom();
        PlayerData.SetLobbyHost(true);
        InstanceFinder.TransportManager.Transport.StartConnection(true);
        var lobbyConductorGo = Instantiate(Instance.netLobbyConductor).gameObject;
        InstanceFinder.ServerManager.Spawn(lobbyConductorGo);
        JoinLobbyLocal();
    }

    public static void JoinLobbyLocal()
    {
        if (PlayerData.PlayerID == 0)
        {
            PlayerData.SetPlayerIDFromRandom();
            PlayerData.SetLobbyHost(false);
        }
        Multipass mp = InstanceFinder.TransportManager.GetTransport<Multipass>();
        mp.SetClientTransport<Tugboat>();
        if (!PlayerData.DisplayName.Contains('#'))
            PlayerData.SetDisplayName($"{PlayerData.DisplayName}#{PlayerData.PlayerID % 10000}");
        InstanceFinder.TransportManager.Transport.StartConnection(false);
        SceneManager.LoadScene("NetLobby");
    }
}
