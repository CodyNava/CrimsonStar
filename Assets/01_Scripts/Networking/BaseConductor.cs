using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine.SceneManagement;

public abstract class BaseConductor<T> : NetworkSingleton<T> where T : NetworkSingleton<T>
{
    public abstract string ConductedSceneName { get; }
    
    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(GetComponent<T>());

        if (!IsServerInitialized) return;

        SceneManager.OnClientPresenceChangeStart += OnClientSceneChange;
        OnNetworkStarted();
    }

    private void OnClientSceneChange(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene.name != ConductedSceneName) return;
        if (args.Added)
        {
            ProcessClientAddition(args.Connection, args.Scene);
        }
        else
        {
            ProcessClientRemoval(args.Connection, args.Scene);
        }
    }
    
    public void MoveToScene(NetworkObject[] objectsToMove = null)
    {
        SceneLoadData loadData = new SceneLoadData(ConductedSceneName);
        loadData.PreferredActiveScene = new PreferredScene(loadData.SceneLookupDatas[0]);
        if (objectsToMove != null)
        {
            loadData.MovedNetworkObjects = objectsToMove;
        }
        SceneManager.LoadGlobalScenes(loadData);
        OnLoadConductedScene();
    }

    public void MoveToScene<TConductor>(BaseConductor<TConductor> previous = null, NetworkObject[] objectsToMove = null) where TConductor : BaseConductor<TConductor>
    {
        SceneLoadData loadData = new SceneLoadData(ConductedSceneName);
        loadData.PreferredActiveScene = new PreferredScene(loadData.SceneLookupDatas[0]);
        if (objectsToMove != null)
        {
            loadData.MovedNetworkObjects = objectsToMove;
        }
        SceneManager.LoadGlobalScenes(loadData);
        OnLoadConductedScene();

        if (previous == null) return;
        previous.OnUnloadConductedScene();
        SceneUnloadData unloadData = new SceneUnloadData(previous.ConductedSceneName);
        SceneManager.UnloadGlobalScenes(unloadData);
    }

    public virtual void OnUnloadConductedScene(){}
    public virtual void OnLoadConductedScene(){}
    
    
    protected virtual void OnNetworkStarted() { }

    public virtual void ProcessClientAddition(NetworkConnection connection, Scene scene) { }
    
    public virtual void ProcessClientRemoval(NetworkConnection connection, Scene scene) { }
    
    protected virtual void OnNetworkStopped() { }

    public override void OnStopNetwork()
    {
        InstanceFinder.UnregisterInstance<T>();
        
        if (!IsServerInitialized) return;
        
        SceneManager.OnClientPresenceChangeStart -= OnClientSceneChange;
        OnNetworkStopped();
    }
}