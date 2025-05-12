using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Dropdown resolutionDropdown;
    Resolution[] resolutions;
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider brightnessSlider;
    [SerializeField] LiftGammaGain gamma;
    [SerializeField] Volume volume;

    const string masterVolume = "MasterVolume";
    const string gammaValue = "GammaValue";

    private void Awake()
    {
        volume.profile.TryGet<LiftGammaGain>(out gamma);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(masterVolume, Mathf.Log10(volumeSlider.value) * 20);
        Save();
    }

    public void AdjustGamma(float value)
    {
        gamma.gamma.value = new Vector4(1f, 1f, 1f, brightnessSlider.value);
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

        if (!PlayerPrefs.HasKey("MasterVolume") || !PlayerPrefs.HasKey("GammaValue"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 0.5f);
            PlayerPrefs.SetFloat("GammaValue", 0.5f);
            Load();
        }

        else
        {
            Load();
        }
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
        PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
        PlayerPrefs.SetFloat("GammaValue", brightnessSlider.value);
    }

    private void Load()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat("GammaValue");
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }
}