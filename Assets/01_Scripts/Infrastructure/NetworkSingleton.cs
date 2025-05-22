using FishNet.Object;
using UnityEngine;

/// <summary>
/// Inherit from this to create a Singleton, which is initialized once a Scene containing it on a GameObject loads.
///
/// This is a parallel to the regular SceneSingleton except for NetworkObjects
/// </summary>
/// <typeparam name="T">The component that should be a Singleton.</typeparam>
public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
{
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