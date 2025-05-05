using UnityEngine;

/// <summary>
/// Inherit from this to create a Singleton, which is initialized once a Scene containing it on a GameObject loads.
/// Don't Destroy On Load makes the Singleton persist between scenes.
/// </summary>
/// <typeparam name="T">The component that should be a Singleton.</typeparam>
public class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
{
    [SerializeField] protected bool dontDestroyOnLoad = true;

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    Debug.LogWarning($"No {typeof(T).Name} found in scene!");
                    return null;
                }

                if (_instance.dontDestroyOnLoad)
                {
                    _instance.transform.SetParent(null);
                    DontDestroyOnLoad(_instance.gameObject);
                }

                _instance.Init();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            _instance = null;
        }
    }

    /// <summary>
    /// Override this to handle initialization logic specific to the class you're making a singleton of.
    /// </summary>
    protected virtual void Init() { }
}