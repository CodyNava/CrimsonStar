using FishNet;
using TMPro;
using UnityEngine;

public class GameSettingsHost : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown gameModeDropdown;
    [SerializeField] private TMP_Dropdown teamModeDropdown;
    [SerializeField] private TMP_Text gameModeText;
    [SerializeField] private TMP_Text teamModeText;
    [SerializeField] private TMP_Text startingCurrencyText;
    [SerializeField] private TMP_Text currencyPerRoundText;

    public void Initialize()
    {
        gameModeDropdown.gameObject.SetActive(PlayerData.IsLobbyHost);
        gameModeText.gameObject.SetActive(!PlayerData.IsLobbyHost);
        teamModeDropdown.gameObject.SetActive(PlayerData.IsLobbyHost);
        teamModeText.gameObject.SetActive(!PlayerData.IsLobbyHost);
        
        UpdateGameSettingsDisplay(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = NetGameModeID.DefaultMode
        });
    }

    public void SetTeamMode(NetTeamModeID teamMode)
    {
        teamModeText.text = teamMode.ToString();
    }
    
    public void UpdateGameSettingsDisplay(NetLobbyBroadcasts.SetGameMode settings)
    {
        gameModeText.text = settings.GameMode.ToString();
        startingCurrencyText.text = DataProvider.Instance.GameModeConfig.Descriptions[settings.GameMode].BaseCurrency.ToString();
        currencyPerRoundText.text = DataProvider.Instance.GameModeConfig.Descriptions[settings.GameMode].CurrencyAddedPerRound.ToString();
    }

    public void UpdateGameMode(int selectedGameMode)
    {
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = (NetGameModeID)selectedGameMode
        });
    }

    public void UpdateTeamMode(int selectedTeamMode)
    {
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.SetTeamMode()
        {
            TeamMode = (NetTeamModeID)selectedTeamMode
        });
    }
}