using HeathenEngineering.SteamworksIntegration.API;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetLobbyConductor lobbyConductor;
    [SerializeField] string playScene, lobbyScene;
    [SerializeField] private Button onlineButton, localHostButton, localJoinButton;
    private EventSystem _eventSystem;

    private void OnEnable()
    {
        PlayerData.SetLobbyHost(false);
        _eventSystem = FindFirstObjectByType<EventSystem>();
        if (App.Initialized)
        {
            onlineButton.gameObject.SetActive(true);
            _eventSystem.SetSelectedGameObject(onlineButton.gameObject);
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