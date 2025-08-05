using System.Collections;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Transporting;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Fishy = FishySteamworks.FishySteamworks;

public class NetGameBootstrapper : SceneSingleton<NetGameBootstrapper>
{
    [Header("Dependencies")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Fishy steamTransport;
    [SerializeField] private string mainMenuSceneName;
    [Header("Conductors")]
    [SerializeField] private NetLobbyConductor netLobbyConductor;

    private NetLobbyConductor _lobbyConductorInstance;
    
    protected Callback<LobbyCreated_t> SteamLobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> SteamLobbyJoinRequested;
    protected Callback<LobbyEnter_t> SteamLobbyEnter;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Additive);
    }
    
    private void OnEnable()
    {
        if(InstanceFinder.TransportManager.Transport)
            InstanceFinder.TransportManager.Transport.OnClientConnectionState += OnConnectionState;
    }

    private void OnDisable()
    {
        if(InstanceFinder.TransportManager != null && InstanceFinder.TransportManager.Transport != null)
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
        SetOnlineLobbySettings();

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 9);
    }

    public void CreatePrivateLobby()
    {
        // Create local session
        PlayerData.SetPlayerIDFromRandom();
        PlayerData.SetLobbyHost(true);
        InstanceFinder.TransportManager.Transport.StartConnection(true);
        _lobbyConductorInstance = Instantiate(Instance.netLobbyConductor);
        var lobbyConductorGo = _lobbyConductorInstance.gameObject;
        InstanceFinder.ServerManager.Spawn(lobbyConductorGo);

        // Setup local session settings
        _lobbyConductorInstance.RoundCount = 99;
        _lobbyConductorInstance.RefundModuleID = NetRefundModuleID.Full;
        _lobbyConductorInstance.EditorTimerDuration = 999;
        _lobbyConductorInstance.FriendlyFireID = NetFirendlyFireID.Off;

        SetTrainingsGroundSettings();

        // On join local session
        Multipass mp = InstanceFinder.TransportManager.GetTransport<Multipass>();
        mp.SetClientTransport<Tugboat>();
        InstanceFinder.TransportManager.Transport.StartConnection(false);
        
        // Await client connection
        StartCoroutine(AwaitClientConnectionIsValid());
    }

    private IEnumerator AwaitClientConnectionIsValid()
    {
        while (!InstanceFinder.ClientManager.Connection.IsActive)
            yield return new WaitForSeconds(0.1f);
        
        _lobbyConductorInstance.AddPlayer(InstanceFinder.ClientManager.Connection,new NetLobbyBroadcasts.PlayerIdentified
        {
            PlayerID = PlayerData.PlayerID,
            DisplayName = PlayerData.DisplayName,
            IsHost = PlayerData.IsLobbyHost
        });
        _lobbyConductorInstance.S_SetGameMode(NetGameModeID.TestingMode);

        _lobbyConductorInstance.PrepareGame();
        InstanceFinder.GetInstance<NetShipEditorConductor>().MoveToScene(_lobbyConductorInstance.Players);
        if(SceneManager.GetSceneByName("BootstrappingScene").IsValid()) SceneManager.UnloadSceneAsync("BootstrappingScene");
        SceneManager.UnloadSceneAsync("MainMenu");
        yield return null;
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
        SetOnlineLobbySettings();
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

    private static void SetOnlineLobbySettings()
    {
        InstanceFinder.GetInstance<NetShipEditorConductor>().SkipAnnouncer = false;
        InstanceFinder.GetInstance<NetGameplayConductor>().SetGameplayScene("NetGameplayScene");
    }

    private void SetTrainingsGroundSettings()
    {
        InstanceFinder.GetInstance<NetShipEditorConductor>().SkipAnnouncer = true;
        InstanceFinder.GetInstance<NetGameplayConductor>().SetGameplayScene("TrainingsGround");
    }
}
