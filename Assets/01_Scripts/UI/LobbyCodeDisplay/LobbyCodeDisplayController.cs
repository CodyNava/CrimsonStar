using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCodeDisplayController : MonoBehaviour
{
    [SerializeField] private Button _copyLobbyCodeBtn;
    [SerializeField] private AdvButton _showLobbyCodeBtn;
    [SerializeField] private TextMeshProUGUI _lobbyCodeLabel;
    [SerializeField] private TextMeshProUGUI _informationLabel;
    [SerializeField] private float _infoLabelFadout = 3f;
    [SerializeField] private char _codeObscureChar = '*';

    private float _infoLabelFade = 0f;
    private string _lobbyCode;
    private string _obscuredLobbyCode;

    private void Update()
    {
        if (_infoLabelFade < 0f) return;
        _infoLabelFade -= Time.deltaTime;
        if (_infoLabelFade <= 0f)
        {
            ClearInfoLabel();
        }
    }

    private void OnEnable()
    {
        _copyLobbyCodeBtn.onClick.AddListener(OnCopyLobbyCodeBtnClicked);
        _showLobbyCodeBtn.onPressed.AddListener(OnShowLobbyCode);
        _showLobbyCodeBtn.onReleased.AddListener(OnHideLobbyCode);
        ClearInfoLabel();
        
        _lobbyCode = PlayerData.CurrentLobbyID.m_SteamID.ToString();

        for (int i = 0; i < _lobbyCode.Length; i++)
        {
            _obscuredLobbyCode += _codeObscureChar;
        }

        _lobbyCodeLabel.text = _obscuredLobbyCode;
    }

    private void OnDisable()
    {
        _copyLobbyCodeBtn.onClick.RemoveListener(OnCopyLobbyCodeBtnClicked);
        _showLobbyCodeBtn.onPressed.RemoveListener(OnShowLobbyCode);
        _showLobbyCodeBtn.onReleased.RemoveListener(OnHideLobbyCode);
    }

    private void ClearInfoLabel()
    {
        _informationLabel.text = "";
        _informationLabel.gameObject.SetActive(false);
        _infoLabelFade = -1f;
    }
    
    private void SetInfoLabel(string info)
    {
        _informationLabel.text = info;
        _informationLabel.gameObject.SetActive(true);
        _infoLabelFade = _infoLabelFadout;
    }

    private void OnCopyLobbyCodeBtnClicked()
    {
        GUIUtility.systemCopyBuffer = _lobbyCode;
        SetInfoLabel("Code copied to clipboard.");
    }
    
    private void OnShowLobbyCode()
    {
        _lobbyCodeLabel.text = _lobbyCode;
    }

    private void OnHideLobbyCode()
    {
        _lobbyCodeLabel.text = _obscuredLobbyCode;
    }
}
