using System.Collections.Generic;
using FishNet;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSettingsHost : MonoBehaviour
{
    [Header("General")]
    private EventSystem _eventSystem;
    
    [SerializeField] private GameObject[] currentSelectedTabList;
    private int _currentSelectedTabIndex;

    [SerializeField] private GameObject playerLb;
    [SerializeField] private GameObject playerRb;
    [SerializeField] private GameObject gameModeSettingsLb;
    [SerializeField] private GameObject gameModeSettingsRb;
    [SerializeField] private GameObject customSettingsLb;
    [SerializeField] private GameObject customSettingsRb;

    [SerializeField] private Button leave;

    [Header("Player")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject team1;
    [SerializeField] private GameObject team2;

    //  FOR KICK private NetMatchPlayer _playerID;

    [SerializeField] private GameObject switchTeamController;
    [SerializeField] private GameObject switchTeamKeyboard;
    [SerializeField] private GameObject inviteController;
    [SerializeField] private GameObject inviteKeyboard;
    [SerializeField] private GameObject kickController;
    [SerializeField] private GameObject kickKeyboard;

    [Header("GameMode")]
    [SerializeField] private GameObject gameMode;
    [SerializeField] private TMP_Text gameModeText;
    [SerializeField] private Button nextGameMode;
    [SerializeField] private Button previousGameMode;
    private readonly string[] _gameMode = { "Free For All", "Team Mode" };
    private int _currentSelectedMode;
    public NetTeamModeID CurrentSelectedTeamMode => (NetTeamModeID)_currentSelectedMode;
    
    [Header("Resources")] 
    [SerializeField] private GameObject resource;
    [SerializeField] private TMP_Text resourceModeText;
    [SerializeField] private Button nextResourceMode;
    [SerializeField] private Button previousResourceMode;
    private readonly string[] _resourceMode = { "Easy", "Default", "Hardcore", "Testing", "Custom" };
    private int _currentResourceMode;

    [Header("Rounds")]
    [SerializeField] private GameObject rounds;
    [SerializeField] private TMP_Text roundsText;
    [SerializeField] private Button increaseRounds;
    [SerializeField] private Button decreaseRounds;

    [Header("Starting Currency")]
    [SerializeField] private GameObject startingCurrency;

    [SerializeField] private TMP_Text startingCurrencyText;
    [SerializeField] private Button increaseStartingCurrency;
    [SerializeField] private Button decreaseStartingCurrency;
    private List<int> _startCurrencyList;
    private int _startCurrencyIndex;

    [Header("Currency Per Round")]
    [SerializeField] private GameObject currencyPerRound;

    [SerializeField] private TMP_Text currencyPerRoundText;
    [SerializeField] private Button increaseCurrencyPerRound;
    [SerializeField] private Button decreaseCurrencyPerRound;
    private List<int> _currencyPerRoundList;
    private int _currencyPerRoundIndex;

    [Header("Module Refund")]
    [SerializeField] private GameObject moduleRefund;

    [SerializeField] private TMP_Text moduleRefundText;
    [SerializeField] private Button increaseModuleRefund;
    [SerializeField] private Button decreaseModuleRefund;

    public void Initialize()
    {
        _eventSystem = EventSystem.current;

        _startCurrencyList = new List<int>();
        _currencyPerRoundList = new List<int>();

        for (var i = 0; i < 4; i++)
        {
            _startCurrencyList.Add(DataProvider.GetStartingCurrency((NetGameModeID)i));
            _currencyPerRoundList.Add(DataProvider.GetCurrencyAddedPerRound((NetGameModeID)i));
        }

        nextResourceMode.gameObject.SetActive(PlayerData.IsLobbyHost);
        previousResourceMode.gameObject.SetActive(PlayerData.IsLobbyHost);

        nextGameMode.gameObject.SetActive(PlayerData.IsLobbyHost);
        previousGameMode.gameObject.SetActive(PlayerData.IsLobbyHost);

        /*increaseRounds.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseRounds.gameObject.SetActive(PlayerData.IsLobbyHost);*/

        increaseStartingCurrency.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseStartingCurrency.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseCurrencyPerRound.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseCurrencyPerRound.gameObject.SetActive(PlayerData.IsLobbyHost);

        /*increaseModuleRefund.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseModuleRefund.gameObject.SetActive(PlayerData.IsLobbyHost);*/

        _eventSystem.SetSelectedGameObject(player1);

        UpdateGameSettingsDisplay(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = NetGameModeID.DefaultMode,
            BaseCurrency = DataProvider.GetStartingCurrency((NetGameModeID)_currentResourceMode),
            CurrencyAddedPerRound = DataProvider.GetCurrencyAddedPerRound((NetGameModeID)_currentResourceMode)
        });

        _currentResourceMode = 1;
        _startCurrencyIndex = 1;
        _currencyPerRoundIndex = 1;
        
        resourceModeText.text = _resourceMode[_currentResourceMode];
     
        gameModeText.text = "Free For All";
        SelectTab(_currentSelectedTabIndex);
    }

    private void Update()
    {
        if (!InputManager.Instance.IsGamepadUsed)
        {
            switchTeamController.SetActive(false);
            inviteController.SetActive(false);
            kickController.SetActive(false);

            switchTeamKeyboard.SetActive(_currentSelectedMode.Equals(1));
            inviteKeyboard.SetActive(true);
            kickKeyboard.SetActive(PlayerData.IsLobbyHost);
            SelectTab(3);
            return;
        }

        switchTeamController.SetActive(_currentSelectedMode.Equals(1));
        inviteController.SetActive(true);
        kickController.SetActive(PlayerData.IsLobbyHost);

        switchTeamKeyboard.SetActive(false);
        inviteKeyboard.SetActive(false);
        kickKeyboard.SetActive(false);
        
        if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
        {
            leave.onClick.Invoke();
        }

        if (Keybinds.Actions.UI.Submit.WasPerformedThisFrame())
        {
            //TODO: Implement Team Switching
        }

        if (!PlayerData.IsLobbyHost) return;

        if (_eventSystem.currentSelectedGameObject.IsUnityNull()) return;

        if (PlayerData.IsLobbyHost)
        {
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
                    previousResourceMode.onClick.Invoke();
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

            if (Keybinds.Actions.UI.SwapTabLeft.WasPerformedThisFrame())
            {
                _currentSelectedTabIndex = (_currentSelectedTabIndex == 0)
                    ? currentSelectedTabList.Length - 1
                    : _currentSelectedTabIndex - 1;
                SelectTab(_currentSelectedTabIndex);
            }

            if (Keybinds.Actions.UI.SwapTabRight.WasPerformedThisFrame())
            {
                _currentSelectedTabIndex = (_currentSelectedTabIndex + 1) % currentSelectedTabList.Length;
                SelectTab(_currentSelectedTabIndex);
            }
        }
    }

    #region Settings

    private void SelectTab(int index)
    {
        switch (index)
        {
            case 0:
                _eventSystem.SetSelectedGameObject(player1);
                playerLb.SetActive(true);
                playerRb.SetActive(true);
                gameModeSettingsLb.SetActive(false);
                gameModeSettingsRb.SetActive(false);
                customSettingsLb.SetActive(false);
                customSettingsRb.SetActive(false);
                break;
            
            case 1:
                _eventSystem.SetSelectedGameObject(gameMode);
                playerLb.SetActive(false);
                playerRb.SetActive(false);
                gameModeSettingsLb.SetActive(false);
                gameModeSettingsRb.SetActive(false);
                customSettingsLb.SetActive(true);
                customSettingsRb.SetActive(true);
                switchTeamController.SetActive(false);
                inviteController.SetActive(true);
                kickController.SetActive(false);
                break;

            case 2:
                _eventSystem.SetSelectedGameObject(resource);
                playerLb.SetActive(false);
                playerRb.SetActive(false);
                gameModeSettingsLb.SetActive(true);
                gameModeSettingsRb.SetActive(true);
                customSettingsLb.SetActive(false);
                customSettingsRb.SetActive(false);
                switchTeamController.SetActive(false);
                inviteController.SetActive(true);
                kickController.SetActive(false);
                break;
            case 3:
                playerLb.SetActive(false);
                playerRb.SetActive(false);
                gameModeSettingsLb.SetActive(false);
                gameModeSettingsRb.SetActive(false);
                customSettingsLb.SetActive(false);
                customSettingsRb.SetActive(false);
                break;
        }
    }

   public void UpdateGameSettingsDisplay(NetLobbyBroadcasts.SetGameMode settings)
   {
        resourceModeText.text = _resourceMode[(int)settings.GameMode];
        startingCurrencyText.text = settings.BaseCurrency.ToString();
        currencyPerRoundText.text = settings.CurrencyAddedPerRound.ToString();
    }

   public void UpdateGameModeDisplay(NetLobbyBroadcasts.SetTeamMode teamMode)
   {
       _currentSelectedMode = (int)teamMode.TeamMode;
       
       if (_currentSelectedMode == 1)
       {
           team1.SetActive(true);
           team2.SetActive(true);
       }

       if (_currentSelectedMode == 0)
       {
           team1.SetActive(false);
           team2.SetActive(false);
       }

       gameModeText.text = _gameMode[_currentSelectedMode];
   }

    private void UpdateResourceMode(int selectedGameMode)
    {
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.SetGameMode
        {
            GameMode = (NetGameModeID)selectedGameMode,
            BaseCurrency = DataProvider.GetStartingCurrency((NetGameModeID)_currentResourceMode),
            CurrencyAddedPerRound = DataProvider.GetCurrencyAddedPerRound((NetGameModeID)_currentResourceMode)
        });

        resourceModeText.text = _resourceMode[selectedGameMode];
    }

    public void NextResources()
    {
        _currentResourceMode++;
        _currentResourceMode %= 4;

        _currencyPerRoundIndex = _currentResourceMode;
        _startCurrencyIndex = _currentResourceMode;

        DataProvider.Instance.customGameMode.BaseCurrency =
            DataProvider.GetStartingCurrency((NetGameModeID)_currentResourceMode);
        DataProvider.Instance.customGameMode.CurrencyAddedPerRound =
            DataProvider.GetCurrencyAddedPerRound((NetGameModeID)_currentResourceMode);
        UpdateResourceMode(_currentResourceMode);
    }

    public void PreviousResources()
    {
        if (_currentResourceMode == 0)
            _currentResourceMode = 3;
        else
            _currentResourceMode--;

        _currencyPerRoundIndex = _currentResourceMode;
        _startCurrencyIndex = _currentResourceMode;

        DataProvider.Instance.customGameMode.BaseCurrency =
            DataProvider.GetStartingCurrency((NetGameModeID)_currentResourceMode);
        DataProvider.Instance.customGameMode.CurrencyAddedPerRound =
            DataProvider.GetCurrencyAddedPerRound((NetGameModeID)_currentResourceMode);
        UpdateResourceMode(_currentResourceMode);
    }

    private void UpdateGameMode(int selectedTeamMode)
    {
        var bc = new NetLobbyBroadcasts.SetTeamMode
        {
            TeamMode = (NetTeamModeID)selectedTeamMode
        };
        InstanceFinder.ClientManager.Broadcast(bc);

        UpdateGameModeDisplay(bc);
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
        _startCurrencyIndex++;
        _startCurrencyIndex %= _startCurrencyList.Count;

        Debug.Log("starting" + _startCurrencyIndex);
        DataProvider.Instance.customGameMode.BaseCurrency = _startCurrencyList[_startCurrencyIndex];
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void DecreaseStartingCurrency()
    {
        if (_startCurrencyIndex == 0)
        {
            _startCurrencyIndex = _startCurrencyList.Count - 1;
        }
        else
        {
            _startCurrencyIndex--;
        }

        Debug.Log("starting" + _startCurrencyIndex);
        DataProvider.Instance.customGameMode.BaseCurrency = _startCurrencyList[_startCurrencyIndex];
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void IncreasePerRoundCurrency()
    {
        _currencyPerRoundIndex++;
        _currencyPerRoundIndex %= _currencyPerRoundList.Count;

        Debug.Log("Round" + _currencyPerRoundIndex);
        DataProvider.Instance.customGameMode.CurrencyAddedPerRound = _currencyPerRoundList[_currencyPerRoundIndex];
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void DecreasePerRoundCurrency()
    {
        if (_currencyPerRoundIndex == 0)
        {
            _currencyPerRoundIndex = _currencyPerRoundList.Count - 1;
        }
        else
        {
            _currencyPerRoundIndex--;
        }

        Debug.Log("Round" + _currencyPerRoundIndex);
        DataProvider.Instance.customGameMode.CurrencyAddedPerRound = _currencyPerRoundList[_currencyPerRoundIndex];
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