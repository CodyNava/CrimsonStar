using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneAudioManager : MonoBehaviour
{
    [SerializeField] private EventReference mainMenu;
    [SerializeField] private EventReference inGameMusic;
    [SerializeField] private EventReference killAnnouncer;
    private EventInstance _mainMenuInstance;
    private EventInstance _inGameMusicInstance;
    private EventInstance _killAnnouncerInstance;

    public static SceneAudioManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        _mainMenuInstance = RuntimeManager.CreateInstance(mainMenu);
        _inGameMusicInstance = RuntimeManager.CreateInstance(inGameMusic);
        _killAnnouncerInstance = RuntimeManager.CreateInstance(killAnnouncer);
    }

    public void StartInGameMusic()
    {
        _inGameMusicInstance.start();
    }

    public void StopInGameMusic()
    {
        _inGameMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ToggleInGameMusic()
    {
        _inGameMusicInstance.getPaused(out bool paused);
        _inGameMusicInstance.setPaused(!paused);
    }


    public void IncreaseMusicProgress()
    {
        _inGameMusicInstance.getParameterByName("Music-progress", out float value);
        _inGameMusicInstance.setParameterByName("Music-progress", value + 1);
    }

    public void DecreaseMusicProgress()
    {
        _inGameMusicInstance.getParameterByName("Music-progress", out float value);
        _inGameMusicInstance.setParameterByName("Music-progress", value - 1);
    }

    public void ResetMusicProgress()
    {
        _inGameMusicInstance.setParameterByName("Music-progress", 0);
    }

    public void StartMainMusic()
    {
        _mainMenuInstance.start();
    }

    public void StopMainMusic()
    {
        _mainMenuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ToggleMainMusic()
    {
        _mainMenuInstance.getPaused(out bool paused);
        _mainMenuInstance.setPaused(!paused);
    }

    public void PlayKillAnnouncer(int killCount)
    {
        _killAnnouncerInstance.setParameterByName("Kill_count", killCount);
        _killAnnouncerInstance.start();
    }
}