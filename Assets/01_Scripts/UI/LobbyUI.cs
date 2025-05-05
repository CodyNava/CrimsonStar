using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] nameList;
    [SerializeField] private Button startGameButton;
    
    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<LobbyBroadcasts.PlayerListUpdate>(OnPlayerListUpdate);
    }

    private void OnPlayerListUpdate(LobbyBroadcasts.PlayerListUpdate msg, Channel channel)
    {
        if (SteamPlayer.IsLobbyHost)
        {
            startGameButton.gameObject.SetActive(true);
        }
        
        ClearNames();

        for (int i = 0; i < msg.Players.Length; i++)
        {
            nameList[i].text = msg.Players[i].playerDisplayName;
        }
    }

    private void ClearNames()
    {
        foreach (TMP_Text label in nameList)
        {
            label.text = string.Empty;
        }
    }

    public void StartGame()
    {
        InstanceFinder.ClientManager.Broadcast(new LobbyBroadcasts.GameStartRequested());
    }
    
    public void LeaveLobby()
    {
        NetSteamBootstrapper.LeaveLobby();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<LobbyBroadcasts.PlayerListUpdate>(OnPlayerListUpdate);
    }
}
