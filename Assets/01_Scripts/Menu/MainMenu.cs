using System;
using HeathenEngineering.SteamworksIntegration.API;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetLobbyConductor lobbyConductor;
    [SerializeField] string playScene, lobbyScene;
    [SerializeField] private JoinLobbyPopUpController _pupUpController;
    [SerializeField] private Button onlineButton, localHostButton, localJoinButton, showJoinPopUpButton;
    private EventSystem _eventSystem;

    private void OnEnable()
    {
        showJoinPopUpButton.onClick.AddListener(OnShowJoinLobbyPopUp);
        
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

    private void OnDisable()
    {
        showJoinPopUpButton.onClick.RemoveListener(OnShowJoinLobbyPopUp);
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

    private void OnShowJoinLobbyPopUp()
    {
        _pupUpController.ShowPopUp();
    }

    public void Quit()
    {
        Application.Quit();
    }
}