using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSettingsHost : MonoBehaviour
{
    private EventSystem _eventSystem;
    [SerializeField] private GameObject ready;

    [Header("GameMode")] [SerializeField] private GameObject gameMode;
    [SerializeField] private TMP_Text gameModeText;
    [SerializeField] private Button nextGameMode;
    [SerializeField] private Button previousGameMode;
    private readonly string[] _gameMode = { "Free For All", "Team Mode" };
    private int _currentSelectedMode;

    [Header("Resources")] [SerializeField] private GameObject resource;
    [SerializeField] private TMP_Text resourceModeText;
    [SerializeField] private Button nextResourceMode;
    [SerializeField] private Button previousResourceMode;
    private readonly string[] _resourceMode = { "Default", "Easy", "Hardcore", "Testing", "Custom" };
    private int _currentResourceMode;

    [Header("Rounds")] [SerializeField] private GameObject rounds;
    [SerializeField] private TMP_Text roundsText;
    [SerializeField] private Button increaseRounds;
    [SerializeField] private Button decreaseRounds;

    [Header("Starting Currency")] [SerializeField]
    private GameObject startingCurrency;

    [SerializeField] private TMP_Text startingCurrencyText;
    [SerializeField] private Button increaseStartingCurrency;
    [SerializeField] private Button decreaseStartingCurrency;

    [Header("Currency Per Round")] [SerializeField]
    private GameObject currencyPerRound;

    [SerializeField] private TMP_Text currencyPerRoundText;
    [SerializeField] private Button increaseCurrencyPerRound;
    [SerializeField] private Button decreaseCurrencyPerRound;

    [Header("Module Refund")] [SerializeField]
    private GameObject moduleRefund;

    [SerializeField] private TMP_Text moduleRefundText;
    [SerializeField] private Button increaseModuleRefund;
    [SerializeField] private Button decreaseModuleRefund;

    public void Initialize()
    {
        _eventSystem = EventSystem.current;
        
        nextResourceMode.gameObject.SetActive(PlayerData.IsLobbyHost);
        previousResourceMode.gameObject.SetActive(PlayerData.IsLobbyHost);

        nextGameMode.gameObject.SetActive(PlayerData.IsLobbyHost);
        previousGameMode.gameObject.SetActive(PlayerData.IsLobbyHost);

        /*increaseRounds.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseRounds.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseStartingCurrency.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseStartingCurrency.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseCurrencyPerRound.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseCurrencyPerRound.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseModuleRefund.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseModuleRefund.gameObject.SetActive(PlayerData.IsLobbyHost);*/

        if (PlayerData.IsLobbyHost)
            _eventSystem.SetSelectedGameObject(gameMode);
        else
            _eventSystem.SetSelectedGameObject(ready);

        UpdateGameSettingsDisplay(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = NetGameModeID.DefaultMode
        });
        resourceModeText.text = "Default";
        gameModeText.text = "Free For All";
    }

    private void Update()
    {
        if (!PlayerData.IsLobbyHost) return;
            
        if (_eventSystem.currentSelectedGameObject.Equals(resource) && Keybinds.Actions.UI.Submit.WasPressedThisFrame())
        {
            _eventSystem.SetSelectedGameObject(startingCurrency);
            DataProvider.Instance.customGameMode.BaseCurrency =
                DataProvider.GetStartingCurrency((NetGameModeID)_currentResourceMode);
            DataProvider.Instance.customGameMode.CurrencyAddedPerRound =
                DataProvider.GetCurrencyAddedPerRound((NetGameModeID)_currentResourceMode);
        }

        if (_eventSystem.currentSelectedGameObject.Equals(startingCurrency) ||
            _eventSystem.currentSelectedGameObject.Equals(currencyPerRound) ||
            _eventSystem.currentSelectedGameObject.Equals(moduleRefund))
        {
            if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
            {
                _eventSystem.SetSelectedGameObject(resource);
            }
        }

        if (Keybinds.Actions.UI.Increase.WasPressedThisFrame())
        {
            if (_eventSystem.currentSelectedGameObject.Equals(gameMode))
            {
                nextGameMode.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(resource))
            {
                nextResourceMode.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(rounds))
            {
                increaseRounds.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(startingCurrency))
            {
                increaseStartingCurrency.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(currencyPerRound))
            {
                increaseCurrencyPerRound.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(moduleRefund))
            {
                increaseModuleRefund.onClick.Invoke();
            }
        }

        if (Keybinds.Actions.UI.Decrease.WasPressedThisFrame())
        {
            if (_eventSystem.currentSelectedGameObject.Equals(gameMode))
            {
                previousGameMode.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(resource))
            {
                previousGameMode.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(rounds))
            {
                decreaseRounds.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(startingCurrency))
            {
                decreaseStartingCurrency.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(currencyPerRound))
            {
                decreaseCurrencyPerRound.onClick.Invoke();
            }

            if (_eventSystem.currentSelectedGameObject.Equals(moduleRefund))
            {
                decreaseModuleRefund.onClick.Invoke();
            }
        }
    }

    #region Settings

    public void SetTeamMode(NetTeamModeID teamMode)
    {
        gameModeText.text = teamMode.ToString();
    }

    public void UpdateGameSettingsDisplay(NetLobbyBroadcasts.SetGameMode settings)
    {
        startingCurrencyText.text = DataProvider.GetStartingCurrency(settings.GameMode).ToString();
        currencyPerRoundText.text = DataProvider.GetCurrencyAddedPerRound(settings.GameMode).ToString();
    }

    private void UpdateResourceMode(int selectedGameMode)
    {
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = (NetGameModeID)selectedGameMode
        });
        resourceModeText.text = _resourceMode[selectedGameMode];
    }

    public void NextResources()
    {
        _currentResourceMode++;
        _currentResourceMode %= 5;

        UpdateResourceMode(_currentResourceMode);
    }

    public void PreviousResources()
    {
        if (_currentResourceMode == 0)
            _currentResourceMode = 4;
        else
            _currentResourceMode--;
        UpdateResourceMode(_currentResourceMode);
    }

    private void UpdateGameMode(int selectedTeamMode)
    {
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.SetTeamMode()
        {
            TeamMode = (NetTeamModeID)selectedTeamMode
        });
        gameModeText.text = _gameMode[selectedTeamMode];
    }

    public void NextGameMode()
    {
        _currentSelectedMode++;
        _currentSelectedMode %= 2;

        UpdateGameMode(_currentSelectedMode);
    }

    public void PreviousGameMode()
    {
        if (_currentSelectedMode == 0)
            _currentSelectedMode = 1;
        else
            _currentSelectedMode--;
        UpdateGameMode(_currentSelectedMode);
    }

    public void IncreaseRounds()
    {
    }

    public void DecreaseRounds()
    {
    }

    public void IncreaseStartingCurrency()
    {
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void DecreaseStartingCurrency()
    {
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void IncreasePerRoundCurrency()
    {
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void DecreasePerRoundCurrency()
    {
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void IncreaseModuleRefund()
    {
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void DecreaseModuleRefund()
    {
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    #endregion
}