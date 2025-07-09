using _01_Scripts.GameState.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject deathScreenUi;
    [SerializeField] private Image brightness;
    [SerializeField] private SettingsBehaviour settingsBehaviour;

    [SerializeField] private bool paused = false;

    private void Awake()
    {
        settingsBehaviour.Load();
        CombatLose_GameState.onEnterState += CombatLose_GameState_onEnterState;
        CombatLose_GameState.onExitState += CombatLose_GameState_onExitState;
    }

    private void OnDestroy()
    {
        CombatLose_GameState.onEnterState -= CombatLose_GameState_onEnterState;
        CombatLose_GameState.onExitState -= CombatLose_GameState_onExitState;
    }

    private void CombatLose_GameState_onExitState()
    {
        deathScreenUi.SetActive(false);
    }

    private void CombatLose_GameState_onEnterState(_01_Scripts.GameState.GameStateController obj)
    {
        deathScreenUi.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        paused = true;
        panel.SetActive(true);
        pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        paused = false;
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
    }

    public void BackToMenu()
    {
        Resume();
        NetModuleWeaponGroupData.ClearAllWeaponGroupKeys();
        ShipEditorHealthOverlay.ClearHealthMap();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
