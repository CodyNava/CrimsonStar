using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneAudioManager : MonoBehaviour
{
    [SerializeField] private EventReference MainMenu;
    [SerializeField] private EventReference InGameMusic;
    private EventInstance _mainMenuInstance;
    private EventInstance _inGameMusicInstance;

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

        _mainMenuInstance = RuntimeManager.CreateInstance(MainMenu);
        _inGameMusicInstance = RuntimeManager.CreateInstance(InGameMusic);
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
}