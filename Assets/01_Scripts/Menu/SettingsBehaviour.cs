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

    [SerializeField] Volume volume;
    [SerializeField] LiftGammaGain gamma;
    [SerializeField] Slider gammaSlider;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Image brightness;

    const string masterVolume = "MasterVolume";
    const string musicVolume = "MusicVolume";
    const string sfxVolume = "SFXVolume";
    const string uiVolume = "UIVolume";
    const string gammaValue = "GammaValue";
    const string brightnessValue = "BrightnessValue";

    private void Awake()
    {
        volume.profile.TryGet<LiftGammaGain>(out gamma);
    }

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

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        //Application.targetFrameRate = (int)resolution.refreshRateRatio.value;
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
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetFloat("UIVolume", uiSlider.value);
        PlayerPrefs.SetFloat("GammaValue", gammaSlider.value);
        PlayerPrefs.SetFloat("BrightnessValue", brightnessSlider.value);
    }

    private void Load()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        uiSlider.value = PlayerPrefs.GetFloat("UIVolume", 0.5f);
        gammaSlider.value = PlayerPrefs.GetFloat("GammaValue", 0.5f);
        brightnessSlider.value = PlayerPrefs.GetFloat("BrightnessValue", 0.5f);
    }
}