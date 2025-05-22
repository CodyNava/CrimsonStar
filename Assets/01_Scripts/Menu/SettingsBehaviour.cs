using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsBehaviour : MonoBehaviour
{
    [SerializeField] private Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    [SerializeField] private AudioMixer master;

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

    private void Awake()
    {
        volume.profile.TryGet(out gamma);
    }

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption = $"{resolutions[i].width} x {resolutions[i].height} @{Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value)}";
            resolutionOptions.Add(resolutionOption);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                Math.Abs(resolutions[i].refreshRateRatio.value - Screen.currentResolution.refreshRateRatio.value) < 0.0001f)
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
        QualitySettings.vSyncCount = QualitySettings.vSyncCount == 1 ? 0 : 1;
        Save();
    }
    #endregion

    #region Sound
    public void MasterVolume()
    {
        master.SetFloat(masterVolume, Mathf.Log10(masterSlider.value) * 20);
        Save();
    }

    public void MusicVolume()
    {
        master.SetFloat(musicVolume, Mathf.Log10(musicSlider.value) * 20);
        Save();
    }

    public void SFXVolume()
    {
        master.SetFloat(sfxVolume, Mathf.Log10(sfxSlider.value) * 20);
        Save();
    }

    public void UIVolume()
    {
        master.SetFloat(uiVolume, Mathf.Log10(uiSlider.value) * 20);
        Save();
    }

    public void AnnouncerVolume()
    {
        master.SetFloat(announcerVolume, Mathf.Log10(announcerSlider.value) * 20);
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
        masterSlider.value = PlayerPrefs.GetFloat(masterVolume, 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat(musicVolume, 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat(sfxVolume, 0.5f);
        uiSlider.value = PlayerPrefs.GetFloat(uiVolume, 0.5f);
        announcerSlider.value = PlayerPrefs.GetFloat(announcerVolume, 0.5f);
        gammaSlider.value = PlayerPrefs.GetFloat(gammaValue, 0.5f);
        brightnessSlider.value = PlayerPrefs.GetFloat(brightnessValue, 0.5f);
        toggle.isOn = PlayerPrefs.GetInt(vSync, 1) == 1;
    }
}