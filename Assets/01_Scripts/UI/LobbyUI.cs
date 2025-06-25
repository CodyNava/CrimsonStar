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

    private bool _ready;
    
    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerListUpdate>(OnPlayerListUpdate);
        InstanceFinder.ClientManager.RegisterBroadcast<NetLobbyBroadcasts.SetGameMode>(OnGameModeChanged);
        hostSettings.Initialize();
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

        for (int i = 0; i < msg.Players.Length; i++)
        {
            playerPlates[i].UpdateDisplay(msg.Players[i], msg.TeamMode);
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
    }
}
