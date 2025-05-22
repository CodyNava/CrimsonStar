using System;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider announcerSlider;

    [SerializeField] private Volume volume;
    [SerializeField] private LiftGammaGain gamma;
    [SerializeField] private Slider gammaSlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image brightness;
    [SerializeField] private Toggle toggle;

    private const string masterVolume = "MasterVolume";
    private const string musicVolume = "MusicVolume";
    private const string sfxVolume = "SFXVolume";
    private const string uiVolume = "UIVolume";
    private const string announcerVolume = "AnnouncerVolume";
    private const string gammaValue = "GammaValue";
    private const string brightnessValue = "BrightnessValue";
    private const string vSync = "VSync";

    FMOD.Studio.Bus _masterBus;
    FMOD.Studio.Bus _musicBus;
    FMOD.Studio.Bus _sfxBus;
    FMOD.Studio.Bus _uiBus;
    FMOD.Studio.Bus _announcerBus;
    float masterbusVolume, musicbusVolume, sfxbusVolume, uibusvolume, announcerbusvolume;

    private void Awake()
    {
        volume.profile.TryGet(out gamma);
    }

    private void Start()
    {
        _masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        _musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        _sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        _uiBus = FMODUnity.RuntimeManager.GetBus("bus:/UI");
        _announcerBus = FMODUnity.RuntimeManager.GetBus("bus:/Voice");

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption =
                $"{resolutions[i].width} x {resolutions[i].height} @{Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value)}";
            resolutionOptions.Add(resolutionOption);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                Math.Abs(resolutions[i].refreshRateRatio.value - Screen.currentResolution.refreshRateRatio.value) <
                0.0001f)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Load();
    }

    #region Graphics

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        //Application.targetFrameRate = (int)resolution.refreshRateRatio.value;
    }

    public void AdjustBrightness()
    {
        brightness.color = new Color(0f, 0f, 0f, brightnessSlider.value);
        Save();
    }

    public void AdjustGamma()
    {
        gamma.gamma.value = new Vector4(1f, 1f, 1f, gammaSlider.value);
        Save();
    }

    public void SetVsync()
    {
        QualitySettings.vSyncCount = toggle.isOn ? 1 : 0;
        Save();
    }

    #endregion

    #region Sound

    public void MasterVolume()
    {
        var result = _masterBus.setVolume(masterSlider.value);
        Debug.Log(result);
        _masterBus.getVolume(out masterbusVolume);
        Debug.Log("Volume is: " + masterbusVolume);
        Save();
    }

    public void MusicVolume()
    {
        var result = _musicBus.setVolume(musicSlider.value);
        Debug.Log(result);
        _musicBus.getVolume(out musicbusVolume);
        Debug.Log("Volume is: " + musicbusVolume);
        Save();
    }

    public void SFXVolume()
    {
        var result = _sfxBus.setVolume(sfxSlider.value);
        Debug.Log(result);
        _sfxBus.getVolume(out sfxbusVolume);
        Debug.Log("Volume is: " + sfxbusVolume);
        Save();
    }

    public void UIVolume()
    {
        var result = _uiBus.setVolume(uiSlider.value);
        Debug.Log(result);
        _uiBus.getVolume(out uibusvolume);
        Debug.Log("Volume is: " + uibusvolume);
        Save();
    }

    public void AnnouncerVolume()
    {
        var result = _announcerBus.setVolume(announcerSlider.value);
        Debug.Log(result);
        _announcerBus.getVolume(out announcerbusvolume);
        Debug.Log("Volume is: " + announcerbusvolume);
        Save();
    }

    private void ApplyVolume(Bus bus, float newVolume)
    {
        bus.setVolume(newVolume);
        Save();
    }

    #endregion

    private void Save()
    {
        PlayerPrefs.SetFloat(masterVolume, masterSlider.value);
        PlayerPrefs.SetFloat(musicVolume, musicSlider.value);
        PlayerPrefs.SetFloat(sfxVolume, sfxSlider.value);
        PlayerPrefs.SetFloat(uiVolume, uiSlider.value);
        PlayerPrefs.SetFloat(announcerVolume, announcerSlider.value);
        PlayerPrefs.SetFloat(gammaValue, gammaSlider.value);
        PlayerPrefs.SetFloat(brightnessValue, brightnessSlider.value);
        PlayerPrefs.SetInt(vSync, QualitySettings.vSyncCount);
    }

    private void Load()
    {
        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(masterVolume, 0.5f));
        musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(musicVolume, 0.5f));
        sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(sfxVolume, 0.5f));
        uiSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(uiVolume, 0.5f));
        announcerSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(announcerVolume, 0.5f));
        gammaSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(gammaValue, 0.5f));
        brightnessSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(brightnessValue, 0.5f));
        toggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(vSync, 1) == 1);
        QualitySettings.vSyncCount = toggle.isOn ? 1 : 0;
    }
}