using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using _01_Scripts.GameState.States;
using _01_Scripts.GameState;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject settingsMenuUI;
    [SerializeField] GameObject bridge;
    [SerializeField] GameObject deathScreenUi;
    
    [SerializeField] bool paused = false;

    private void Awake()
    {
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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
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
        bridge.SetActive(false);
        paused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }
       
    public void Resume()
    {
        bridge.SetActive(true);
        paused = false;
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void BackToMenu()
    {
        Resume();
        GameStateController.Instance.ChangeState(new MainMenu_GameState());
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
