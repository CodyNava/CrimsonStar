using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject resolutionSettings;
    [SerializeField] private Button backButton;
    [SerializeField] private Button teeHeeButton;
    [SerializeField] private EventSystem eventSystem;
    
    private bool _settingsActive;

    public void ToggleSettings()
    {
        _settingsActive = !_settingsActive;
    }

    private void Update()
    {
        if (Keybinds.Actions.UI.PauseGame.WasPressedThisFrame() && !_settingsActive)
        {
            //settingsMenu.SetActive(true);
            teeHeeButton.onClick.Invoke();
            InputManager.DisableGameControls();
            _settingsActive = true;
            return;
        }

        if (Keybinds.Actions.UI.PauseGame.WasPressedThisFrame() && _settingsActive)
        {
            backButton.onClick.Invoke();
            InputManager.EnableGameControls();
        }
    }
}