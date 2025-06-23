using _01_Scripts.GameState.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject settingsMenuUI;
    [SerializeField] GameObject deathScreenUi;
    [SerializeField] Image brightness;

    [SerializeField] bool paused = false;

    private void Awake()
    {
        brightness.color = new Color(0f, 0f, 0f, PlayerPrefs.GetFloat("BrightnessValue"));
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
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        paused = false;
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void BackToMenu()
    {
        Resume();
        //GameStateController.Instance.ChangeState(new MainMenu_GameState());
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
