using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSettingsMenuInput : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private EventSystem eventSystem;
    
    [SerializeField] private Button audioButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject resolution;
    [SerializeField] private GameObject frameCap;
    [SerializeField] private GameObject vSync;
    
    [SerializeField] private Button increaseResolution;
    [SerializeField] private Button decreaseResolution;
    [SerializeField] private Button increaseFrameCap;
    [SerializeField] private Button decreaseFrameCap;
    [SerializeField] private Button increaseVsync;
    [SerializeField] private Button decreaseVsync;
    

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

            if (Keybinds.Actions.UI.Decrease.WasPressedThisFrame())
            {
                if (eventSystem.currentSelectedGameObject.Equals(resolution))
                {
                    decreaseResolution.onClick.Invoke();
                }

                if (eventSystem.currentSelectedGameObject.Equals(frameCap))
                {
                    decreaseFrameCap.onClick.Invoke();
                }
                if (eventSystem.currentSelectedGameObject.Equals(vSync))
                {
                    decreaseVsync.onClick.Invoke();
                }
            }

            if (Keybinds.Actions.UI.Increase.WasPressedThisFrame())
            {
                if (eventSystem.currentSelectedGameObject.Equals(resolution))
                {
                    increaseResolution.onClick.Invoke();
                }

                if (eventSystem.currentSelectedGameObject.Equals(frameCap))
                {
                    increaseFrameCap.onClick.Invoke();
                }

                if (eventSystem.currentSelectedGameObject.Equals(vSync))
                {
                    increaseVsync.onClick.Invoke();
                }
            }
        }
    }
}