using TMPro;
using UnityEngine;

public class GameSettingsHost : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown startingCurrencyDropdown;
    [SerializeField] private TMP_Dropdown currencyPerRoundDropdown;
    [SerializeField] private TMP_Text startingCurrencyText;
    [SerializeField] private TMP_Text currencyPerRoundText;

    public void Initialize()
    {
        bool canEdit = SteamPlayer.IsLobbyHost;
        if (canEdit)
        {
            startingCurrencyDropdown.enabled = true;
            currencyPerRoundDropdown.enabled = true;
            startingCurrencyText.enabled = false;
            currencyPerRoundText.enabled = false;
        }
        else
        {
            startingCurrencyDropdown.enabled = false;
            currencyPerRoundDropdown.enabled = false;
            startingCurrencyText.enabled = true;
            currencyPerRoundText.enabled = true;
        }
    }
}