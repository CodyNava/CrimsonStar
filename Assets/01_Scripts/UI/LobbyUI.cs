using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private PlayerPlateDisplay[] playerPlates;
    [SerializeField] private Button startGameButton;
    
    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<NetLobbyBroadcasts.PlayerListUpdate>(OnPlayerListUpdate);
    }

    private void OnPlayerListUpdate(NetLobbyBroadcasts.PlayerListUpdate msg, Channel channel)
    {
        if (SteamPlayer.IsLobbyHost)
        {
            startGameButton.gameObject.SetActive(true);
        }
        
        ClearNames();

        for (int i = 0; i < msg.Players.Length; i++)
        {
            playerPlates[i].UpdateDisplay(msg.Players[i]);
        }
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
        NetSteamBootstrapper.LeaveLobby();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<NetLobbyBroadcasts.PlayerListUpdate>(OnPlayerListUpdate);
    }
}
