using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSettingsMenuInput : MonoBehaviour
{
    private bool _cooldownStarted;

    [Tooltip("Scroll Speed in SECONDS. Use values smaller than 1. PLEASE I BEG YOU.")]
    [Range(0, 1)]
    [SerializeField] private float scrollSpeedSeconds;

    [SerializeField] private GameObject settingsContainer;
    private EventSystem _eventSystem;

    [SerializeField] private Button audioButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button settingsBackButton;

    [Header("Graphics")]
    [SerializeField] private GameObject graphicsContainer;
    [SerializeField] private GameObject resolution;
    [SerializeField] private GameObject frameCap;
    [SerializeField] private GameObject vSync;
    [SerializeField] private GameObject brightness;
    [SerializeField] private GameObject gamma;
    
    [SerializeField] private Button increaseResolution;
    [SerializeField] private Button decreaseResolution;
    [SerializeField] private Button increaseFrameCap;
    [SerializeField] private Button decreaseFrameCap;
    [SerializeField] private Button increaseVsync;
    [SerializeField] private Button decreaseVsync;
    [SerializeField] private Button increaseBrightness;
    [SerializeField] private Button decreaseBrightness;
    [SerializeField] private Button increaseGamma;
    [SerializeField] private Button decreaseGamma;

    [Header("Sound")]
    [SerializeField] private GameObject audioContainer;
    [SerializeField] private GameObject master;
    [SerializeField] private GameObject music;
    [SerializeField] private GameObject sfx;
    [SerializeField] private GameObject voice;
    [SerializeField] private GameObject ui;

    [SerializeField] private Button increaseMaster;
    [SerializeField] private Button decreaseMaster;
    [SerializeField] private Button increaseMusic;
    [SerializeField] private Button decreaseMusic;
    [SerializeField] private Button increaseSfx;
    [SerializeField] private Button decreaseSfx;
    [SerializeField] private Button increaseVoice;
    [SerializeField] private Button decreaseVoice;
    [SerializeField] private Button increaseUi;
    [SerializeField] private Button decreaseUi;

    private void Awake()
    {
        settingsContainer = GameObject.Find("SettingsCanvas");
        _eventSystem = FindFirstObjectByType<EventSystem>();
    }

    private void Update()
    {
        if (settingsContainer.activeSelf)
        {
            if (Keybinds.Actions.UI.RightTrigger.WasPressedThisFrame())
            {
                if (audioContainer.activeSelf)
                {
                    graphicsButton.onClick.Invoke();
                    _eventSystem.SetSelectedGameObject(resolution);
                }
                else
                {
                    audioButton.onClick.Invoke();
                    _eventSystem.SetSelectedGameObject(master);
                }
            }

            if (Keybinds.Actions.UI.LeftTrigger.WasPressedThisFrame())
            {
                if (graphicsContainer.activeSelf)
                {
                    audioButton.onClick.Invoke();
                    _eventSystem.SetSelectedGameObject(master);
                }
                else
                {
                    graphicsButton.onClick.Invoke();
                    _eventSystem.SetSelectedGameObject(resolution);
                }
            }

            //TODO: Implement BackButton on Other Scripts
            /*if (Keybinds.Actions.UI.Cancel.WasPressedThisFrame())
            {
                settingsBackButton.onClick.Invoke();
            }*/

            if (Keybinds.Actions.UI.Decrease.IsPressed() && !_cooldownStarted)
            {
                if (_eventSystem.currentSelectedGameObject.Equals(resolution))
                {
                    decreaseResolution.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(frameCap))
                {
                    decreaseFrameCap.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(vSync))
                {
                    decreaseVsync.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(brightness))
                {
                    decreaseBrightness.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(gamma))
                {
                    decreaseGamma.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(master))
                {
                    decreaseMaster.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(music))
                {
                    decreaseMusic.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(sfx))
                {
                    decreaseSfx.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(voice))
                {
                    decreaseVoice.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(ui))
                {
                    decreaseUi.onClick.Invoke();
                }

                StartCoroutine(HoldCooldown());
            }

            if (Keybinds.Actions.UI.Increase.IsPressed() && !_cooldownStarted)
            {
                if (_eventSystem.currentSelectedGameObject.Equals(resolution))
                {
                    increaseResolution.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(frameCap))
                {
                    increaseFrameCap.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(vSync))
                {
                    increaseVsync.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(brightness))
                {
                    increaseBrightness.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(gamma))
                {
                    increaseGamma.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(master))
                {
                    increaseMaster.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(music))
                {
                    increaseMusic.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(sfx))
                {
                    increaseSfx.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(voice))
                {
                    increaseVoice.onClick.Invoke();
                }

                if (_eventSystem.currentSelectedGameObject.Equals(ui))
                {
                    increaseUi.onClick.Invoke();
                }

                StartCoroutine(HoldCooldown());
            }
        }
    }

    private IEnumerator HoldCooldown()
    {
        _cooldownStarted = true;
        yield return new WaitForSeconds(scrollSpeedSeconds);
        _cooldownStarted = false;
    }
}