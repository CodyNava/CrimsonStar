using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuBackButtonHandler : MonoBehaviour
{
    private EventSystem _eventSystem;
    [SerializeField] private SettingsBehaviour settingsBehaviour;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject warningPopUp;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private GameObject apply;
    [SerializeField] private PauseMenu pauseMenu;

    private void Awake()
    {
        _eventSystem = EventSystem.current;
    }

    private void Update()
    {
        if (warningPopUp.activeSelf && Keybinds.Actions.UI.Submit.WasPressedThisFrame())
        {
            saveButton.onClick.Invoke();
        }
            
        if (warningPopUp.activeSelf && Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
        {
            discardButton.onClick.Invoke();
        }
    }

    public void BackButtonHandler()
    {
            if (settingsBehaviour.unsavedChanges)
            {
                warningPopUp.SetActive(true);
                _eventSystem.sendNavigationEvents = false;
                return;
            }
            
            _eventSystem.sendNavigationEvents = true;
            if (mainMenuCanvas != null)
            {
                mainMenuCanvas.SetActive(true);
            }
            apply.SetActive(false);

            if (pauseMenu != null)
            {
                pauseMenu.ToggleSettings();
            }
            settingsCanvas.SetActive(false);
            if (settingsButton != null)
            {
                _eventSystem.SetSelectedGameObject(settingsButton);
            }
            else
            {
                _eventSystem.SetSelectedGameObject(null);
            }
    }
}