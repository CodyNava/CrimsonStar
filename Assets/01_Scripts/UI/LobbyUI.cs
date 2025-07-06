using FishNet;
using FishNet.Transporting;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private PlayerPlateDisplay[] playerPlates;
    [SerializeField] private GameSettingsHost hostSettings;
    [SerializeField] private Button startGameButton, readyButton;
    [SerializeField] private TMP_Text readyButtonText;
    [SerializeField] private Image brightness;
    [SerializeField] private SwitchTeamsButton switchTeamsButton;

    private NetTeamModeID _currentTeamMode;
    
    private bool _ready;

    private void Awake()
    {
        brightness.color = new Color(0f, 0f, 0f, PlayerPrefs.GetFloat("BrightnessValue"));
    }
    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerListUpdate>(OnPlayerListUpdate);
        InstanceFinder.ClientManager.RegisterBroadcast<NetLobbyBroadcasts.SetGameMode>(OnGameModeChanged);
        InstanceFinder.ClientManager.RegisterBroadcast<NetLobbyBroadcasts.SetTeamMode>(OnTeamModeChanged);
        hostSettings.Initialize();
        switchTeamsButton.SetPlayerID(PlayerData.PlayerID);
    }

    private void OnTeamModeChanged(NetLobbyBroadcasts.SetTeamMode msg, Channel channel)
    {
        hostSettings.UpdateGameModeDisplay(msg);
    }

    private void OnGameModeChanged(NetLobbyBroadcasts.SetGameMode msg, Channel channel)
    {
        hostSettings.UpdateGameSettingsDisplay(msg);
    }

    private void OnPlayerListUpdate(NetLobbyBroadcasts.PlayerListUpdate msg, Channel channel)
    {
        if (PlayerData.IsLobbyHost)
        {
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            readyButton.gameObject.SetActive(true);
        }
        
        ClearNames();
        
        int teamAIndice = 0;
        int teamBIndice = 0;
        for (int i = 0; i < msg.Players.Length; i++)
        {
            if (hostSettings.CurrentSelectedTeamMode == NetTeamModeID.TeamMode)
            {
                switch (msg.Players[i].playerTeamID)
                {
                    case NetTeamID.Team1:
                        playerPlates[teamAIndice].UpdateDisplay(msg.Players[i], msg.TeamMode);
                        teamAIndice++;
                        break;
                    case NetTeamID.Team2:
                        playerPlates[4+teamBIndice].UpdateDisplay(msg.Players[i], msg.TeamMode);
                        teamBIndice++;
                        break;
                }
            }
            else
            {
                playerPlates[i].UpdateDisplay(msg.Players[i], msg.TeamMode);
            }
            
            if (msg.Players[i].playerID == PlayerData.PlayerID)
            {
                switchTeamsButton.UpdateTeamID(msg.Players[i].playerTeamID);
            }
        }
    }

    public void ToggleReady()
    {
        _ready = !_ready;
        readyButtonText.text = _ready ? "Unready" : "Ready";
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.SetReadyState
        {
            ReadyState = _ready
        });
    }

    private void ClearNames()
    {
        foreach (PlayerPlateDisplay display in playerPlates)
        {
            display.ResetDisplay();
        }
    }

    public void StartGame()
    {
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.GameStartRequested());
    }
    
    public void LeaveLobby()
    {
        if (PlayerData.CurrentLobbyID != CSteamID.Nil)
            NetGameBootstrapper.LeaveLobby();
        else
        {
            NetGameBootstrapper.LeaveLobbyLocal();
        }
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<NetLobbyBroadcasts.PlayerListUpdate>(OnPlayerListUpdate);
        InstanceFinder.ClientManager.UnregisterBroadcast<NetLobbyBroadcasts.SetGameMode>(OnGameModeChanged);
        InstanceFinder.ClientManager.UnregisterBroadcast<NetLobbyBroadcasts.SetTeamMode>(OnTeamModeChanged);
    }
}
