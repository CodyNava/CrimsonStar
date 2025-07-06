using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpectatorPlayerPanelEntry : MonoBehaviour
{
    private string _playerName;

    [SerializeField] private TMP_Text _playerNameTMP;
    [SerializeField] private Button _playerButton;
    private SpectatorPlayerPanel _playerPanel;
    private NetBridge _bridge;

    public bool SetEnable
    {
        set => _playerButton.interactable = value;
    }
    
    
    public void Init(string playerName, SpectatorPlayerPanel playerPanel, NetBridge bridge)
    {
        _playerPanel = playerPanel;
        _bridge = bridge;
        UpdatePlayerName(playerName);
    }

    public void UpdatePlayerName(string playerName)
    {
        _playerName = playerName;
        UpdatePlayerNameDisplay();
    }

    public void UpdatePlayerNameDisplay()
    {
        _playerNameTMP.text = _playerName;
    }

    public void OnPlayerPanelBtnClicked()
    {
        _playerPanel.OnPlayerPanelClicked(_bridge);
    }
}
