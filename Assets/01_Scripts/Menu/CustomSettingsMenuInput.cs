using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CustomSettingsMenuInput : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private EventSystem eventSystem;
    
    
    [SerializeField] private Button audioButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        settingsCanvas = GameObject.Find("SettingsCanvas");
        eventSystem = FindFirstObjectByType<EventSystem>();
    }

    private void Update()
    {
        if (settingsCanvas.activeSelf)
        {
            if (Keybinds.Actions.UI.SwapTabRight.WasPressedThisFrame())
            {
                audioButton.onClick.Invoke();
                eventSystem.SetSelectedGameObject(audioButton.gameObject);
            }

            if (Keybinds.Actions.UI.SwapTabLeft.WasPressedThisFrame())
            {
                graphicsButton.onClick.Invoke();
                eventSystem.SetSelectedGameObject(graphicsButton.gameObject);
            }

            if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
            {
                backButton.onClick.Invoke();
            }
            
        }
    }
}