using FishNet;
using TMPro;
using UnityEngine;

public class GameSettingsHost : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown startingCurrencyDropdown;
    [SerializeField] private TMP_Dropdown currencyPerRoundDropdown;
    [SerializeField] private TMP_Text startingCurrencyText;
    [SerializeField] private TMP_Text currencyPerRoundText;

    private int _selectedStartingCurrency;
    private int _selectedCurrencyPerRound;
    
    public void Initialize()
    {
        bool canEdit = SteamPlayer.IsLobbyHost;
        if (canEdit)
        {
            startingCurrencyDropdown.gameObject.SetActive(true);
            currencyPerRoundDropdown.gameObject.SetActive(true);
            startingCurrencyText.gameObject.SetActive(false);
            currencyPerRoundText.gameObject.SetActive(false);
        }
        else
        {
            startingCurrencyDropdown.gameObject.SetActive(false);
            currencyPerRoundDropdown.gameObject.SetActive(false);
            startingCurrencyText.gameObject.SetActive(true);
            currencyPerRoundText.gameObject.SetActive(true);
        }

        _selectedStartingCurrency = int.Parse(startingCurrencyDropdown.options[startingCurrencyDropdown.value].text);
        _selectedCurrencyPerRound = int.Parse(currencyPerRoundDropdown.options[currencyPerRoundDropdown.value].text);
        UpdateHostSettings();
    }

    public void UpdateGameSettingsDisplay(NetLobbyBroadcasts.SetLobbySettings settings)
    {
        startingCurrencyText.text = settings.InitialResourceCount.ToString();
        currencyPerRoundText.text = settings.ResourceGainPerRound.ToString();
    }

    public void OnStartingCurrencyDropdownChanged(int index)
    {
        _selectedStartingCurrency = int.Parse(startingCurrencyDropdown.options[index].text);
        UpdateHostSettings();
    }

    public void OnCurrencyPerRoundDropdownChanged(int index)
    {
        _selectedCurrencyPerRound = int.Parse(currencyPerRoundDropdown.options[index].text);
        UpdateHostSettings();
    }

    private void UpdateHostSettings()
    {
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.SetLobbySettings
        {
            InitialResourceCount = _selectedStartingCurrency,
            ModuleRecycleRate = 1f,
            NumberOfRounds = 3,
            ResourceGainPerRound = _selectedCurrencyPerRound
        });
    }
}