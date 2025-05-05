using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string playScene, lobbyScene;
    [SerializeField] private Button onlineButton;

    private void OnEnable()
    {
        SteamPlayer.SetLobbyHost(false);
        if (App.Initialized)
        {
            onlineButton.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        GameStateController.Instance.ChangeState(new ShipEditor_GameState());
        SceneManager.LoadScene(playScene);
    }
    
    public void CreateLobby()
    {
        SceneManager.LoadScene(lobbyScene);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 9);
    }

    public void Quit()
    {
       Application.Quit();
    }
}
