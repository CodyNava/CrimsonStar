using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Dropdown resolutionDropdown;
    Resolution[] resolutions;
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioMixer audioMixer;

    const string masterVolume = "MasterVolume";

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(masterVolume, Mathf.Log10(volumeSlider.value) * 20);
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

        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 0.5f);
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
        Application.targetFrameRate = (int)resolution.refreshRateRatio.value;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }
}