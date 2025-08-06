using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject resolutionSettings;
    [SerializeField] private Button backButton;
    private EventSystem _eventSystem;
    private bool _settingsActive;

    private void Awake()
    {
        _eventSystem = FindFirstObjectByType<EventSystem>();
    }

    public void ToggleSettings()
    {
        _settingsActive = !_settingsActive;
    }

    private void Update()
    {
        if (Keybinds.Actions.Player.PauseGame.WasPressedThisFrame() && !_settingsActive)
        {
            settingsMenu.SetActive(true);
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(resolutionSettings);
            }
            _settingsActive = true;
            return;
        }

        if (Keybinds.Actions.Player.PauseGame.WasPressedThisFrame() && _settingsActive)
        {
            backButton.onClick.Invoke();
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}