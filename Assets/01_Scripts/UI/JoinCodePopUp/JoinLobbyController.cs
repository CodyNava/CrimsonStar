using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JoinLobbyController : MonoBehaviour
{
    [SerializeField] private Button _joinLobbyBtn;
    [SerializeField] private InputField _lobbyCodeInputField;

    [SerializeField] private TextMeshProUGUI _informationLabel;
    
    private void OnEnable()
    {
        _joinLobbyBtn.onClick.AddListener(OnJoinLobbyBtnClicked);
        _lobbyCodeInputField.onValueChanged.AddListener(OnLobbyCodeValueChanged);
        
        _informationLabel.text = "";
        _joinLobbyBtn.interactable = _lobbyCodeInputField.text.Length > 0;
    }

    private void OnDisable()
    {
        _joinLobbyBtn.onClick.RemoveListener(OnJoinLobbyBtnClicked);
        _lobbyCodeInputField.onValueChanged.RemoveListener(OnLobbyCodeValueChanged);
    }

    public void ClearInputField()
    {
        _lobbyCodeInputField.text = "";
        _joinLobbyBtn.interactable = false;
    }
    
    private void OnJoinLobbyBtnClicked()
    {
        _joinLobbyBtn.interactable = false;
        CSteamID lobbyId = new CSteamID(Convert.ToUInt64(_lobbyCodeInputField.text));
        SteamMatchmaking.JoinLobby(lobbyId);
        _informationLabel.text = "Connecting ...";
    }

    private void OnLobbyCodeValueChanged(string lobbyCode)
    {
        _joinLobbyBtn.interactable = lobbyCode.Length > 0;
    }
}
