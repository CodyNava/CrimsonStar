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
    private NetLobbyConductor _lobbyConductor;

    [SerializeField] private Button leave;
    [SerializeField] private Button players;
    [SerializeField] private GameObject rT;
    [SerializeField] private GameObject lT;
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject back;
    [SerializeField] private Button options;

    [SerializeField] private GameObject playerContainer;
    [SerializeField] private GameObject optionsContainer;

    [Header("Player")] 
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject team1;
    [SerializeField] private GameObject team2;
    [SerializeField] private Button switchTeam;

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
    [SerializeField] private TMP_Text gameModePreview;
    [SerializeField] private Button nextGameMode;
    [SerializeField] private Button previousGameMode;
    private readonly string[] _gameMode = { "Free For All", "Team Mode" };
    private int _currentSelectedMode;
    public NetTeamModeID CurrentSelectedTeamMode => (NetTeamModeID)_currentSelectedMode;

    [Header("Rounds")] 
    [SerializeField] private GameObject rounds;
    [SerializeField] private TMP_Text roundsText;
    [SerializeField] private TMP_Text roundsPreview;
    [SerializeField] private Button increaseRounds;
    [SerializeField] private Button decreaseRounds;
    private readonly int[] _roundsToPlay = { 3, 5, 9 };
    private int _currentRoundsToPlayIndex;

    [Header("Timer")]
    [SerializeField] private GameObject timer;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text timerPreview;
    [SerializeField] private Button increaseTimer;
    [SerializeField] private Button decreaseTimer;
    private List<float> _timerCounter;
    private int _currentSelectedTimeIndex;

    [Header("Friendly Fire")] 
    [SerializeField] private GameObject friendlyFire;
    [SerializeField] private TMP_Text friendlyFireText;
    [SerializeField] private TMP_Text friendlyFirePreview;
    [SerializeField] private Button increaseFriendlyFire;
    [SerializeField] private Button decreaseFriendlyFire;
    private int _currentFriendlyFireModeIndex;

    [Header("Bots")]
    [SerializeField] private GameObject bots;
    [SerializeField] private TMP_Text botsCount;
    [SerializeField] private Button increaseBots;
    [SerializeField] private Button decreaseBots;
    private int _currentSelectedBotCountIndex;

    [Header("Steam Friends Only")]
    [SerializeField] private GameObject steamFriendsOnly;
    [SerializeField] private TMP_Text friendsOnlyText;
    [SerializeField] private Button increaseFriendsOnly;
    [SerializeField] private Button decreaseFriendsOnly;
    private int _currentSelectedFriendsModeIndex;

    [Header("Resources")]
    [SerializeField] private GameObject resource;
    [SerializeField] private GameObject resourceA;
    [SerializeField] private GameObject settingsA;
    [SerializeField] private TMP_Text resourceModeText;
    [SerializeField] private TMP_Text resourceModePreview;
    [SerializeField] private Button nextResourceMode;
    [SerializeField] private Button previousResourceMode;
    private readonly string[] _resourceMode = { "Easy", "Default", "Hardcore", "Testing", "Custom" };
    private int _currentResourceMode;

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
    private int _currentModuleRefund;

    private bool _isInitialized = false;

    public void Initialize()
    {
        _eventSystem = EventSystem.current;
        InstanceFinder.TryGetInstance(out _lobbyConductor);
        _startCurrencyList = new List<int>();
        _currencyPerRoundList = new List<int>();

        _isInitialized = true;

        for (var i = 0; i < 4; i++)
        {
            _startCurrencyList.Add(DataProvider.GetStartingCurrency((NetGameModeID)i));
            _currencyPerRoundList.Add(DataProvider.GetCurrencyAddedPerRound((NetGameModeID)i));
        }

        _timerCounter = new List<float>();

        for (var i = 1; i <= 20; i++)
        {
            _timerCounter.Add(i * 10);
        }

        players.gameObject.SetActive(PlayerData.IsLobbyHost);
        options.gameObject.SetActive(PlayerData.IsLobbyHost);

        nextResourceMode.gameObject.SetActive(PlayerData.IsLobbyHost);
        previousResourceMode.gameObject.SetActive(PlayerData.IsLobbyHost);

        nextGameMode.gameObject.SetActive(PlayerData.IsLobbyHost);
        previousGameMode.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseRounds.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseRounds.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseTimer.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseTimer.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseFriendlyFire.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseFriendlyFire.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseStartingCurrency.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseStartingCurrency.gameObject.SetActive(PlayerData.IsLobbyHost);

        increaseCurrencyPerRound.gameObject.SetActive(PlayerData.IsLobbyHost);
        decreaseCurrencyPerRound.gameObject.SetActive(PlayerData.IsLobbyHost);

        //increaseModuleRefund.gameObject.SetActive(PlayerData.IsLobbyHost);
        //decreaseModuleRefund.gameObject.SetActive(PlayerData.IsLobbyHost);
        
        lT.SetActive(PlayerData.IsLobbyHost && InputManager.Instance.IsGamepadUsed);
        rT.SetActive(PlayerData.IsLobbyHost && InputManager.Instance.IsGamepadUsed);

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
        _currentSelectedTimeIndex = 11;
        _currentFriendlyFireModeIndex = 0;
        _currentRoundsToPlayIndex = 0;
        _currentModuleRefund = 0;

        _startCurrencyIndex = _currentResourceMode;
        _currencyPerRoundIndex = _currentResourceMode;
        _lobbyConductor.EditorTimerDuration = _timerCounter[_currentSelectedTimeIndex];
        _lobbyConductor.FriendlyFireID = (NetFirendlyFireID)_currentFriendlyFireModeIndex;
        _lobbyConductor.RefundModuleID = (NetRefundModuleID)_currentModuleRefund;
        _lobbyConductor.RoundCount = _roundsToPlay[_currentRoundsToPlayIndex];

        friendlyFireText.text = _lobbyConductor.FriendlyFireID.ToString();
        friendlyFirePreview.text = _lobbyConductor.FriendlyFireID.ToString();
        timerText.text = _timerCounter[_currentSelectedTimeIndex].ToString();
        timerPreview.text = _timerCounter[_currentSelectedTimeIndex].ToString();
        resourceModeText.text = _resourceMode[_currentResourceMode];
        resourceModePreview.text = _resourceMode[_currentResourceMode];
       // moduleRefundText.text = _lobbyConductor.RefundModuleID.ToString();

        gameModeText.text = "Free For All";
        gameModePreview.text = "Free For All";
        
        SyncPreviewUI();
    }

    private void Update()
    {
        if (!_isInitialized && InstanceFinder.TryGetInstance(out _lobbyConductor))
        {
            Initialize();
        }
        
        if (!InputManager.Instance.IsGamepadUsed)
        {
            switchTeamController.SetActive(false);
            //inviteController.SetActive(false);
            //kickController.SetActive(false);

            switchTeamKeyboard.SetActive(_currentSelectedMode.Equals(1));
            //inviteKeyboard.SetActive(true);
            //kickKeyboard.SetActive(PlayerData.IsLobbyHost);
            
            lT.SetActive(false);
            rT.SetActive(false);
            start.SetActive(false);
            back.SetActive(false);
            return;
        }
        
        switchTeamController.SetActive(_currentSelectedMode.Equals(1));
        //inviteController.SetActive(true);
        //kickController.SetActive(PlayerData.IsLobbyHost);

        switchTeamKeyboard.SetActive(false);
        //inviteKeyboard.SetActive(false);
        //kickKeyboard.SetActive(false);

        if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
        {
            leave.onClick.Invoke();
        }

        if (Keybinds.Actions.UI.Submit.WasPerformedThisFrame() && playerContainer.activeSelf)
        {
            switchTeam.onClick.Invoke();
        }
        start.SetActive(true);
        back.SetActive(true);
        
        if (!PlayerData.IsLobbyHost) return;

        if (_eventSystem.currentSelectedGameObject.IsUnityNull()) return;
        
        lT.SetActive(true);
        rT.SetActive(true);
        
        
        if (PlayerData.IsLobbyHost)
        {
            if (Keybinds.Actions.UI.SwapTabRight.WasPressedThisFrame())
            {
                if (optionsContainer.activeSelf)
                {
                    players.onClick.Invoke();
                }
                else
                {
                    options.onClick.Invoke();
                }
            }

            if (Keybinds.Actions.UI.SwapTabLeft.WasPressedThisFrame())
            {
                if (playerContainer.activeSelf)
                {
                    options.onClick.Invoke();
                }
                else
                {
                    players.onClick.Invoke();
                }
            }

            if (Keybinds.Actions.UI.Increase.WasPerformedThisFrame())
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

                if (_eventSystem.currentSelectedGameObject.Equals(timer))
                {
                    increaseTimer.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(friendlyFire))
                {
                    increaseFriendlyFire.onClick.Invoke();
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

            if (Keybinds.Actions.UI.Decrease.WasPerformedThisFrame())
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

                if (_eventSystem.currentSelectedGameObject.Equals(timer))
                {
                    decreaseTimer.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(friendlyFire))
                {
                    decreaseFriendlyFire.onClick.Invoke();
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

            if (optionsContainer.activeSelf && _eventSystem.currentSelectedGameObject.Equals(resource) &&
                Keybinds.Actions.UI.Submit.WasPerformedThisFrame())
            {
                _eventSystem.SetSelectedGameObject(startingCurrency);
                settingsA.SetActive(true);
                return;
            }

            if (optionsContainer.activeSelf && (_eventSystem.currentSelectedGameObject.Equals(startingCurrency) ||
                                                _eventSystem.currentSelectedGameObject.Equals(currencyPerRound) ||
                                                _eventSystem.currentSelectedGameObject.Equals(moduleRefund)) &&
                Keybinds.Actions.UI.Submit.WasPerformedThisFrame())
            {
                _eventSystem.SetSelectedGameObject(resource);
                settingsA.SetActive(false);
            }
        }
        timerText.text = _timerCounter[_currentSelectedTimeIndex].ToString();
        timerPreview.text = _timerCounter[_currentSelectedTimeIndex].ToString();
    }

    #region Settings

    private void SyncPreviewUI ()
    {
        var preview = new NetLobbyBroadcasts.PreviewUIElements
        {
            GameMode = (NetTeamModeID)_currentSelectedMode,
            ResourceMode = (NetGameModeID)_currentResourceMode,
            Timer = (int)_timerCounter[_currentSelectedTimeIndex],
            RoundCount = _roundsToPlay[_currentRoundsToPlayIndex],
            FriendlyFireMode = (NetFirendlyFireID)_currentFriendlyFireModeIndex,
        };
        if (InstanceFinder.IsServerStarted)
        {
            NetLobbyConductor.Instance.S_SyncPreview(preview);
        }
    }

    public void UpdatePreviewText(NetLobbyBroadcasts.PreviewUIElements preview)
    {
        gameModePreview.text = _gameMode[(int)preview.GameMode];
        resourceModePreview.text = _resourceMode[(int)preview.ResourceMode];
        timerPreview.text = preview.Timer.ToString();
        roundsPreview.text = preview.RoundCount.ToString();
        friendlyFirePreview.text = preview.FriendlyFireMode.ToString();
    }

    public void UpdateGameSettingsDisplay(NetLobbyBroadcasts.SetGameMode settings)
    {
        resourceModeText.text = _resourceMode[(int)settings.GameMode];
        resourceModePreview.text = _resourceMode[(int)settings.GameMode];
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
        gameModePreview.text = _gameMode[_currentSelectedMode];
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
        resourceModePreview.text = _resourceMode[selectedGameMode];
        SyncPreviewUI();
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
        SyncPreviewUI();
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
        SyncPreviewUI();
    }

    public void PreviousGameMode()
    {
        if (_currentSelectedMode == 0)
            _currentSelectedMode = 1;
        else
            _currentSelectedMode--;
        UpdateGameMode(_currentSelectedMode);
        SyncPreviewUI();
    }

    public void IncreaseRounds()
    {
        _currentRoundsToPlayIndex++;
        _currentRoundsToPlayIndex %= 3;

        _lobbyConductor.RoundCount = _roundsToPlay[_currentRoundsToPlayIndex];
        roundsText.text = _roundsToPlay[_currentRoundsToPlayIndex].ToString();
        roundsPreview.text = _roundsToPlay[_currentRoundsToPlayIndex].ToString();
        SyncPreviewUI();
    }

    public void DecreaseRounds()
    {
        if (_currentRoundsToPlayIndex == 0)
        {
            _currentRoundsToPlayIndex = 2;
        }
        else
        {
            _currentRoundsToPlayIndex--;
        }

        _lobbyConductor.RoundCount = _roundsToPlay[_currentRoundsToPlayIndex];
        roundsText.text = _roundsToPlay[_currentRoundsToPlayIndex].ToString();
        roundsPreview.text = _roundsToPlay[_currentRoundsToPlayIndex].ToString();
        SyncPreviewUI();
    }

    public void IncreaseTimer()
    {
        _currentSelectedTimeIndex++;
        _currentSelectedTimeIndex %= _timerCounter.Count;

        _lobbyConductor.EditorTimerDuration = _timerCounter[_currentSelectedTimeIndex];
        timerText.text = _timerCounter[_currentSelectedTimeIndex].ToString();
        timerPreview.text = _timerCounter[_currentSelectedTimeIndex].ToString();
        SyncPreviewUI();
    }

    public void DecreaseTimer()
    {
        if (_currentSelectedTimeIndex == 0)
        {
            _currentSelectedTimeIndex = _timerCounter.Count - 1;
        }
        else
        {
            _currentSelectedTimeIndex--;
        }

        _lobbyConductor.EditorTimerDuration = _timerCounter[_currentSelectedTimeIndex];
        timerText.text = _timerCounter[_currentSelectedTimeIndex].ToString();
        timerPreview.text = _timerCounter[_currentSelectedTimeIndex].ToString();
        SyncPreviewUI();
    }

    public void IncreaseFriendlyFire()
    {
        _currentFriendlyFireModeIndex++;
        _currentFriendlyFireModeIndex %= 4;
        _lobbyConductor.FriendlyFireID = (NetFirendlyFireID)_currentFriendlyFireModeIndex;
        friendlyFireText.text = _lobbyConductor.FriendlyFireID.ToString();
        friendlyFirePreview.text = _lobbyConductor.FriendlyFireID.ToString();
        SyncPreviewUI();
    }

    public void DecreaseFriendlyFire()
    {
        if (_currentFriendlyFireModeIndex == 0)
        {
            _currentFriendlyFireModeIndex = 3;
        }
        else
        {
            _currentFriendlyFireModeIndex--;
        }

        _lobbyConductor.FriendlyFireID = (NetFirendlyFireID)_currentFriendlyFireModeIndex;
        friendlyFireText.text = _lobbyConductor.FriendlyFireID.ToString();
        friendlyFirePreview.text = _lobbyConductor.FriendlyFireID.ToString();
        SyncPreviewUI();
    }

    public void IncreaseStartingCurrency()
    {
        _startCurrencyIndex++;
        _startCurrencyIndex %= _startCurrencyList.Count;

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

        DataProvider.Instance.customGameMode.BaseCurrency = _startCurrencyList[_startCurrencyIndex];
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void IncreasePerRoundCurrency()
    {
        _currencyPerRoundIndex++;
        _currencyPerRoundIndex %= _currencyPerRoundList.Count;

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

        DataProvider.Instance.customGameMode.CurrencyAddedPerRound = _currencyPerRoundList[_currencyPerRoundIndex];
        UpdateResourceMode((int)NetGameModeID.Custom);
    }

    public void IncreaseModuleRefund()
    {
        _currentModuleRefund++;
        _currentModuleRefund %= 4;
        _lobbyConductor.RefundModuleID = (NetRefundModuleID)_currentModuleRefund;
       // moduleRefundText.text = _lobbyConductor.RefundModuleID.ToString();
    }

    public void DecreaseModuleRefund()
    {
        if (_currentModuleRefund == 0)
        {
            _currentModuleRefund = 3;
        }
        else
        {
            _currentModuleRefund--;
        }
        _lobbyConductor.RefundModuleID = (NetRefundModuleID)_currentModuleRefund;
        //moduleRefundText.text = _lobbyConductor.RefundModuleID.ToString();
    }

    #endregion
}