using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsBehaviour : MonoBehaviour
{
    [SerializeField] Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [SerializeField] AudioMixer master;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider uiSlider;
    [SerializeField] Slider announcerSlider;

    [SerializeField] Volume volume;
    [SerializeField] LiftGammaGain gamma;
    [SerializeField] Slider gammaSlider;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Image brightness;
    [SerializeField] Toggle toggle;

    const string masterVolume = "MasterVolume";
    const string musicVolume = "MusicVolume";
    const string sfxVolume = "SFXVolume";
    const string uiVolume = "UIVolume";
    const string announcerVolume = "AnnouncerVolume";
    const string gammaValue = "GammaValue";
    const string brightnessValue = "BrightnessValue";
    const string vSync = "VSync";

    private void Awake()
    {
        volume.profile.TryGet<LiftGammaGain>(out gamma);
    }

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionoptions = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            //string resolutionoption = resolutions[i].width + " x " + resolutions[i].height;
            string resolutionoption = $"{resolutions[i].width} x {resolutions[i].height} @{Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value)}";
            resolutionoptions.Add(resolutionoption);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionoptions);
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
        if (QualitySettings.vSyncCount == 1)
        {
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            QualitySettings.vSyncCount = 1;
        }
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