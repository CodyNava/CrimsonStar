using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSettingsMenuInput : MonoBehaviour
{
    private bool _cooldownStarted;

    [Tooltip("Scroll Speed in SECONDS. Use values smaller than 1. PLEASE I BEG YOU.")] [Range(0, 1)] [SerializeField]
    private float scrollSpeedSeconds;

    [Header("DO NOT TOUCH")] [SerializeField]
    private GameObject settingsContainer;

    private EventSystem _eventSystem;

    [SerializeField] private Button audioButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button exitButton;

    [Header("Graphics")] [SerializeField] private GameObject graphicsContainer;
    [SerializeField] private GameObject resolution;
    [SerializeField] private GameObject frameCap;
    [SerializeField] private GameObject vSync;
    [SerializeField] private GameObject qualityPref;
    [SerializeField] private GameObject brightness;
    [SerializeField] private GameObject gamma;

    [SerializeField] private Button increaseResolution;
    [SerializeField] private Button decreaseResolution;
    [SerializeField] private Button increaseFrameCap;
    [SerializeField] private Button decreaseFrameCap;
    [SerializeField] private Button increaseVsync;
    [SerializeField] private Button decreaseVsync;
    [SerializeField] private Button increaseQuality;
    [SerializeField] private Button decreaseQuality;
    [SerializeField] private Button increaseBrightness;
    [SerializeField] private Button decreaseBrightness;
    [SerializeField] private Button increaseGamma;
    [SerializeField] private Button decreaseGamma;

    [Header("Sound")] [SerializeField] private GameObject audioContainer;
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

    private void Update()
    {
        if (settingsContainer.activeSelf)
        {
            if (Keybinds.Actions.UI.SwapTabRight.WasPressedThisFrame())
            {
                if (audioContainer.activeSelf)
                {
                    graphicsButton.onClick.Invoke();
                }
                else
                {
                    audioButton.onClick.Invoke();
                }
            }

            if (Keybinds.Actions.UI.SwapTabLeft.WasPressedThisFrame())
            {
                if (graphicsContainer.activeSelf)
                {
                    audioButton.onClick.Invoke();
                }
                else
                {
                    graphicsButton.onClick.Invoke();
                }
            }

            if (Keybinds.Actions.UI.SwitchTeam.WasPressedThisFrame() && applyButton.GameObject().activeSelf)
            {
                applyButton.onClick.Invoke();
            }

            if (Keybinds.Actions.UI.Save.WasPressedThisFrame() && exitButton != null)
            {
                exitButton.onClick.Invoke();
            }

            if (EventSystem.current != null)
            {
                if (Keybinds.Actions.UI.Decrease.IsPressed() && !_cooldownStarted)
                {
                    if (EventSystem.current.currentSelectedGameObject.Equals(resolution))
                    {
                        decreaseResolution.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(frameCap))
                    {
                        decreaseFrameCap.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(vSync))
                    {
                        decreaseVsync.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(qualityPref))
                    {
                        decreaseQuality.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(brightness))
                    {
                        decreaseBrightness.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(gamma))
                    {
                        decreaseGamma.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(master))
                    {
                        decreaseMaster.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(music))
                    {
                        decreaseMusic.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(sfx))
                    {
                        decreaseSfx.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(voice))
                    {
                        decreaseVoice.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(ui))
                    {
                        decreaseUi.onClick.Invoke();
                    }

                    StartCoroutine(HoldCooldown());
                }

                if (Keybinds.Actions.UI.Increase.IsPressed() && !_cooldownStarted)
                {
                    if (EventSystem.current.currentSelectedGameObject.Equals(resolution))
                    {
                        increaseResolution.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(frameCap))
                    {
                        increaseFrameCap.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(vSync))
                    {
                        increaseVsync.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(qualityPref))
                    {
                        increaseQuality.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(brightness))
                    {
                        increaseBrightness.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(gamma))
                    {
                        increaseGamma.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(master))
                    {
                        increaseMaster.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(music))
                    {
                        increaseMusic.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(sfx))
                    {
                        increaseSfx.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(voice))
                    {
                        increaseVoice.onClick.Invoke();
                    }

                    if (EventSystem.current.currentSelectedGameObject.Equals(ui))
                    {
                        increaseUi.onClick.Invoke();
                    }

                    StartCoroutine(HoldCooldown());
                }
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