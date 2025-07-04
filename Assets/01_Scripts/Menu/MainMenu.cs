using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using FishNet;
using FishNet.Transporting.Tugboat;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetLobbyConductor lobbyConductor;
    [SerializeField] string playScene, lobbyScene;
    [SerializeField] private Button onlineButton, localHostButton, localJoinButton;

    private void OnEnable()
    {
        PlayerData.SetLobbyHost(false);
        if (App.Initialized)
        {
            onlineButton.gameObject.SetActive(true);
        }
#if !UNITY_EDITOR
        localHostButton.gameObject.SetActive(false);
        localJoinButton.gameObject.SetActive(false);
#endif
    }

    void Start()
    {
        SceneAudioManager.instance.StartMainMusic();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(playScene);
    }

    public void CreateLobby()
    {
        NetGameBootstrapper.CreateLobby();
    }

    public void CreateLobbyLocal()
    {
        NetGameBootstrapper.CreateLobbyLocal();
    }

    public void JoinLobbyLocal()
    {
        NetGameBootstrapper.JoinLobbyLocal();
    }

    public void Quit()
    {
        Application.Quit();
    }
}