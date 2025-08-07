using System.Collections.Generic;
using FMOD.Studio;
using Steamworks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsBehaviour : MonoBehaviour
{
    [SerializeField] private List <GameObject> controllerPrompts;
    
    [Header("Graphics")]
    [SerializeField] private TMP_Text resolutionText;
    private List<string> _resolutionOptions;
    private List<Resolution> _uniqueResolution;
    private Resolution[] _resolutions;
    private Resolution _resolution;
    private int _currentResolutionIndex;
    private int _uniqueResolutionIndex;

    [SerializeField] private TMP_Text frameCounter;
    private readonly int[] _frameCap = { 30, 60, 90, 120, 144, 180, -1 };
    private int _frameCapIndex;

    [SerializeField] private TMP_Text vSyncMode;
    private readonly string[] _vSync = { "Off", "On" };
    private int _vSyncIndex;

    [SerializeField] private TMP_Text qualityPrefab;
    [SerializeField] List<RenderPipelineAsset> qualityPrefabs;
    private int _qualityPrefIndex;
    [SerializeField] private Volume volume;
    private LiftGammaGain _gamma;
    [SerializeField] private Slider gammaSlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image brightness;
    [SerializeField] private GameObject apply;
    [SerializeField] private GameObject applyPrompt;

    [Header("Sound")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider announcerSlider;

    private const string MasterVolumePref = "MasterVolume";
    private const string MusicVolumePref = "MusicVolume";
    private const string SfxVolumePref = "SFXVolume";
    private const string UiVolumePref = "UIVolume";
    private const string VoiceVolumePref = "AnnouncerVolume";
    private const string GammaValuePref = "GammaValue";
    private const string BrightnessValuePref = "BrightnessValue";
    private const string VSyncPref = "VSync";
    private const string FrameCapPref = "FrameCap";
    private const string ResolutionPref = "Resolution";
    private const string QualityPref = "QualitySettings";

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _sfxBus;
    private Bus _uiBus;
    private Bus _announcerBus;

    private void Awake()
    {
        volume.profile.TryGet(out _gamma);
        apply.SetActive(false);
    }

    public void LeaveGame()
    {
        if (PlayerData.CurrentLobbyID != CSteamID.Nil)
        {
            NetGameBootstrapper.LeaveLobby();
        }
        else
        {
            NetGameBootstrapper.LeaveLobbyLocal();
        }

        SceneAudioManager.instance.StopInGameMusic();
        SceneAudioManager.instance.ResetMusicProgress();
        SceneManager.LoadScene("MainMenu");
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

        Load();
    }

    private void Update()
    {
        foreach (var prompt in controllerPrompts)
        {
            prompt.SetActive(InputManager.Instance.IsGamepadUsed);
        }
        applyPrompt.SetActive(InputManager.Instance.IsGamepadUsed && apply.activeSelf);
    }

    #region Graphics

    public void IncreaseResolution()
    {
        _currentResolutionIndex++;
        _currentResolutionIndex %= _uniqueResolution.Count;

        _resolution = _uniqueResolution[_currentResolutionIndex];
        resolutionText.text = _resolutionOptions[_currentResolutionIndex];
        apply.SetActive(true);
    }

    public void DecreaseResolution()
    {
        if (_currentResolutionIndex == 0)
        {
            _currentResolutionIndex = _uniqueResolution.Count - 1;
        }
        else
        {
            _currentResolutionIndex--;
        }

        _resolution = _uniqueResolution[_currentResolutionIndex];
        resolutionText.text = _resolutionOptions[_currentResolutionIndex];
        apply.SetActive(true);
    }

    public void IncreaseFrameCap()
    {
        _frameCapIndex++;
        _frameCapIndex %= _frameCap.Length;

        frameCounter.text = _frameCap[_frameCapIndex].Equals(-1) ? "Unlimited" : _frameCap[_frameCapIndex].ToString();
        apply.SetActive(true);
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
        apply.SetActive(true);
    }

    public void IncreaseVsync()
    {
        if (_vSyncIndex == 1)
        {
            _vSyncIndex = 0;
        }
        else
        {
            _vSyncIndex++;
        }
        vSyncMode.text = _vSync[_vSyncIndex];
        apply.SetActive(true);
    }

    public void DecreaseVsync()
    {
        if (_vSyncIndex == 0)
        {
            _vSyncIndex = _vSync.Length - 1;
        }
        else
        {
            _vSyncIndex--;
        }
        vSyncMode.text = _vSync[_vSyncIndex];
        apply.SetActive(true);
    }

    public void IncreaseQualityPref()
    {
        if (_qualityPrefIndex == qualityPrefabs.Count -1)
        {
            _qualityPrefIndex = 0;
        }
        else
        {
            _qualityPrefIndex++;
        }
        qualityPrefab.text = qualityPrefabs[_qualityPrefIndex].name;
        apply.SetActive(true);
    }

    public void DecreaseQualityPref()
    {
        if (_qualityPrefIndex == 0)
        {
            _qualityPrefIndex = qualityPrefabs.Count - 1;
        }
        else
        {
            _qualityPrefIndex--;
        }
        qualityPrefab.text = qualityPrefabs[_qualityPrefIndex].name;
        apply.SetActive(true);
    }

    public void AdjustBrightness()
    {
        brightness.color = new Color(0f, 0f, 0f, brightnessSlider.value);
        ApplyGraphicsSlider();
    }

    public void IncreaseBrightness()
    {
        brightnessSlider.value -= 0.05f;
        AdjustBrightness();
    }

    public void DecreaseBrightness()
    {
        brightnessSlider.value += 0.05f;
        AdjustBrightness();
    }

    public void AdjustGamma()
    {
        _gamma.gamma.value = new Vector4(1f, 1f, 1f, gammaSlider.value);
        ApplyGraphicsSlider();
    }

    public void IncreaseGamma()
    {
        gammaSlider.value += 0.05f;
        AdjustGamma();
    }

    public void DecreaseGamma()
    {
        gammaSlider.value -= 0.05f;
        AdjustGamma();
    }

    public void Apply()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, Screen.fullScreen);
        resolutionText.text = _resolutionOptions[_currentResolutionIndex];
        Application.targetFrameRate = _frameCap[_frameCapIndex];
        frameCounter.text = _frameCap[_frameCapIndex].Equals(-1) ? "Unlimited" : _frameCap[_frameCapIndex].ToString();
        QualitySettings.vSyncCount = _vSyncIndex;
        vSyncMode.text = _vSync[_vSyncIndex];
        QualitySettings.renderPipeline = qualityPrefabs[_qualityPrefIndex];
        qualityPrefab.text = qualityPrefabs[_qualityPrefIndex].name;
        Save();
    }
    
    private void ApplyGraphicsSlider()
    {
        PlayerPrefs.SetFloat(GammaValuePref, gammaSlider.value);
        PlayerPrefs.SetFloat(BrightnessValuePref, brightnessSlider.value);
    }

    #endregion

    #region Sound

    public void MasterVolume()
    {
        ApplyVolume(_masterBus, masterSlider.value);
    }

    public void IncreaseMaster()
    {
        masterSlider.value += 0.05f;
        MasterVolume();
    }

    public void DecreaseMaster()
    {
        masterSlider.value -= 0.05f;
        MasterVolume();
    }

    public void MusicVolume()
    {
        ApplyVolume(_musicBus, musicSlider.value);
    }

    public void IncreaseMusic()
    {
        musicSlider.value += 0.05f;
        MusicVolume();
    }

    public void DecreaseMusic()
    {
        musicSlider.value -= 0.05f;
        MusicVolume();
    }

    public void SfxVolume()
    {
        ApplyVolume(_sfxBus, sfxSlider.value);
    }

    public void IncreaseSfx()
    {
        sfxSlider.value += 0.05f;
        SfxVolume();
    }

    public void DecreaseSfx()
    {
        sfxSlider.value -= 0.05f;
        SfxVolume();
    }

    public void UIVolume()
    {
        ApplyVolume(_uiBus, uiSlider.value);
    }

    public void IncreaseUI()
    {
        uiSlider.value += 0.05f;
        UIVolume();
    }

    public void DecreaseUI()
    {
        uiSlider.value -= 0.05f;
        UIVolume();
    }

    public void AnnouncerVolume()
    {
        ApplyVolume(_announcerBus, announcerSlider.value);
    }

    public void IncreaseAnnouncer()
    {
        announcerSlider.value += 0.05f;
        AnnouncerVolume();
    }

    public void DecreaseAnnouncer()
    {
        announcerSlider.value -= 0.05f;
        AnnouncerVolume();
    }

    private void ApplyVolume(Bus bus, float newVolume)
    {
        bus.setVolume(newVolume);
        Save();
    }

    #endregion

    private void Save()
    {
        PlayerPrefs.SetFloat(MasterVolumePref, masterSlider.value);
        PlayerPrefs.SetFloat(MusicVolumePref, musicSlider.value);
        PlayerPrefs.SetFloat(SfxVolumePref, sfxSlider.value);
        PlayerPrefs.SetFloat(UiVolumePref, uiSlider.value);
        PlayerPrefs.SetFloat(VoiceVolumePref, announcerSlider.value);
        PlayerPrefs.SetInt(VSyncPref, _vSyncIndex);
        PlayerPrefs.SetInt(FrameCapPref, _frameCapIndex);
        PlayerPrefs.SetInt(ResolutionPref, _currentResolutionIndex);
        PlayerPrefs.SetInt(QualityPref, _qualityPrefIndex);
    }

    public void Load()
    {
        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MasterVolumePref, 0.5f));
        musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MusicVolumePref, 0.5f));
        sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SfxVolumePref, 0.5f));
        uiSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(UiVolumePref, 0.5f));
        announcerSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(VoiceVolumePref, 0.5f));
        gammaSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GammaValuePref, 0.5f));
        brightnessSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(BrightnessValuePref, 0.5f));
        _vSyncIndex = PlayerPrefs.GetInt(VSyncPref, 1);
        _frameCapIndex = PlayerPrefs.GetInt(FrameCapPref, 1);
        _currentResolutionIndex = PlayerPrefs.GetInt(ResolutionPref, _uniqueResolution.Count - 1);
        _qualityPrefIndex = PlayerPrefs.GetInt(QualityPref, qualityPrefabs.Count - 1);
        MasterVolume();
        MusicVolume();
        SfxVolume();
        UIVolume();
        AnnouncerVolume();
        AdjustBrightness();
        AdjustGamma();
        Apply();
    }
}