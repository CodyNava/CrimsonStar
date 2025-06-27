using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text resolutionText;
    private List<string> _resolutionOptions;
    private List<Resolution> _uniqueResolution;
    private Resolution[] _resolutions;
    private Resolution _resolution;
    private int _currentResolutionIndex;
    private int _uniqueResolutionIndex;

    private int[] _frameCap = {30, 60, 90, 120, 144, 180, -1};
    private int _frameCapIndex;

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
    [SerializeField] private TMP_Text frameCounter;
    
    private const string masterVolume = "MasterVolume";
    private const string musicVolume = "MusicVolume";
    private const string sfxVolume = "SFXVolume";
    private const string uiVolume = "UIVolume";
    private const string announcerVolume = "AnnouncerVolume";
    private const string gammaValue = "GammaValue";
    private const string brightnessValue = "BrightnessValue";
    private const string vSync = "VSync";

    Bus _masterBus;
    Bus _musicBus;
    Bus _sfxBus;
    Bus _uiBus;
    Bus _announcerBus;
    float _masterBusVolume, _musicBusVolume, _sfxBusVolume, _uiBusVolume, _announcerBusVolume;

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

        _resolutions = Screen.resolutions;
        _resolutionOptions = new List<string>();
        _uniqueResolution = new List<Resolution>();
        _uniqueResolutionIndex = 0;
        _currentResolutionIndex = 0;
        
        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolution.IsUnityNull())
            {
                _resolution = Screen.currentResolution;
            }
            string resolutionOption =
                $"{_resolutions[i].width} x {_resolutions[i].height}";
            if (_resolutionOptions.Contains(resolutionOption))
            {
                continue;
            }
            _resolutionOptions.Add(resolutionOption);
            _uniqueResolution.Add(_resolutions[i]);
            
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                _currentResolutionIndex = _uniqueResolutionIndex;
            }
            _uniqueResolutionIndex++;
        }

        _resolution = Screen.currentResolution;
        resolutionText.text = _resolutionOptions[_currentResolutionIndex];
        frameCounter.text = _frameCap[_frameCapIndex].ToString();

        Load();
    }

    public void Apply()
    {
        Screen.SetResolution(_resolution.width, _resolution.height,Screen.fullScreen);
        Application.targetFrameRate = _frameCap[_frameCapIndex];
        Save();
    }

    #region Graphics

    public void IncreaseResolution()
    {
        _currentResolutionIndex++;
        _currentResolutionIndex %= _uniqueResolution.Count;

        _resolution = _uniqueResolution[_currentResolutionIndex];
        resolutionText.text = _resolutionOptions[_currentResolutionIndex];
    }

    public void DecreaseResolution()
    {
        if (_currentResolutionIndex == 0)
        {
            _currentResolutionIndex = _uniqueResolution.Count - 1;
        }
        else
            _currentResolutionIndex--;

        _resolution = _uniqueResolution[_currentResolutionIndex];
        resolutionText.text = _resolutionOptions[_currentResolutionIndex];
    }

    public void IncreaseFrameCap()
    {
        _frameCapIndex++;
        _frameCapIndex %= _frameCap.Length;

        if (_frameCap[_frameCapIndex].Equals(-1))
            frameCounter.text = "Unlimited";
        else
            frameCounter.text = _frameCap[_frameCapIndex].ToString();
    }

    public void DecreaseFrameCap()
    {
        if (_frameCapIndex == 0)
        {
            _frameCapIndex = _frameCap.Length - 1;
            frameCounter.text = "Unlimited";
        }
        else
        {
            _frameCapIndex--;
            frameCounter.text = _frameCap[_frameCapIndex].ToString();
        }
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
        if (QualitySettings.vSyncCount == 0)
        {
            Application.targetFrameRate = _frameCap[_frameCapIndex];
        }
        Save();
    }
    

    #endregion

    #region Sound

    public void MasterVolume()
    {
        ApplyVolume(_masterBus, masterSlider.value);
    }

    public void MusicVolume()
    {
        ApplyVolume(_musicBus, musicSlider.value);
    }

    public void SFXVolume()
    {
        ApplyVolume(_sfxBus, sfxSlider.value);
    }

    public void UIVolume()
    {
        ApplyVolume(_uiBus, uiSlider.value);
    }

    public void AnnouncerVolume()
    {
        ApplyVolume(_announcerBus, announcerSlider.value);
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
        MasterVolume();
        MusicVolume();
        SFXVolume();
        UIVolume();
        SetVsync();
        AnnouncerVolume();
        AdjustBrightness();
        AdjustGamma();
    }
}