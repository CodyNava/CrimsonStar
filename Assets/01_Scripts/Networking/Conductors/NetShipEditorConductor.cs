using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

public class NetShipEditorConductor : NetworkSingleton<NetShipEditorConductor>
{
    [SerializeField] private NetGameplayConductor netGameplayConductor;
    [SerializeField] private NetShipEditorData shipEditorDataPrefab;

    [SerializeField] private int minimumResourceCount;
    [SerializeField] private float shipEditorTimerDuration;
    
    private Dictionary<NetworkConnection, bool> _playersReady = new();
    private readonly SyncTimer _editorTimer = new();

    private NetLobbyConductor _lobbyConductor;
    
    public float TimeRemaining => _editorTimer.Remaining;

    public Dictionary<NetworkConnection, NetShipEditorData> PlayerShipEditors { get; } = new();

    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            _lobbyConductor = InstanceFinder.GetInstance<NetLobbyConductor>();
            SceneManager.OnClientPresenceChangeStart += S_OnSceneChange;
        }
    }

    public override void OnStopNetwork()
    {
        InstanceFinder.UnregisterInstance<NetShipEditorConductor>();
        
        if (IsServerInitialized)
        {
            SceneManager.OnClientPresenceChangeStart -= S_OnSceneChange;
        }
    }

    private void S_OnSceneChange(ClientPresenceChangeEventArgs args)
    {
        int resourceCount = Mathf.Max(_lobbyConductor.S_GetInitialResourceCount(), minimumResourceCount);
        
        if (args.Scene.name == "NetShipEditor" && args.Added)
        {
            if (PlayerShipEditors.ContainsKey(args.Connection) == false)
            {
                var shipEditor = Instantiate(shipEditorDataPrefab);
                shipEditor.S_SetResourceCount(resourceCount);
                ServerManager.Spawn(shipEditor.gameObject, args.Connection, args.Scene);
                PlayerShipEditors.Add(args.Connection, shipEditor);
            }
            else
            {
                PlayerShipEditors[args.Connection].Relink();
            }

            if (_playersReady.Count == 0)
            {
                _editorTimer.StartTimer(shipEditorTimerDuration);
                _editorTimer.OnChange += OnTimerChange;
            }
            
            _playersReady.Add(args.Connection, false);
        }
    }

    private void Update()
    {
        _editorTimer.Update(Time.deltaTime);
    }

    private void OnTimerChange(SyncTimerOperation op, float prev, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished)
        {
            MoveToGameplayScene();
            _editorTimer.OnChange -= OnTimerChange;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void S_SignalReady(Channel channel = Channel.Reliable, NetworkConnection conn = null)
    {
        _playersReady[conn!] = true;

        if (S_AllPlayersReady())
        {
            MoveToGameplayScene();
        }
    }

    private void MoveToGameplayScene()
    {
        SceneLoadData sceneData = new("NetGameplayScene");
        sceneData.PreferredActiveScene = new PreferredScene(sceneData.SceneLookupDatas[0]);
        sceneData.MovedNetworkObjects = PlayerShipEditors.Values.Select(data => data.NetworkObject).ToArray();
        SceneUnloadData unloadData = new("NetShipEditor");
        SceneManager.LoadGlobalScenes(sceneData);
        SceneManager.UnloadGlobalScenes(unloadData);

        if (!InstanceFinder.GetInstance<NetGameplayConductor>())
        {
            GameObject gameConductor = Instantiate(netGameplayConductor).gameObject;
            ServerManager.Spawn(gameConductor);
        }
        
        _playersReady.Clear();
    }

    private bool S_AllPlayersReady()
    {
        return _playersReady.Values.All(ready => ready);
    }

    public void S_SetupNewEditPhase()
    {
        foreach (var shipEditor in PlayerShipEditors.Values)
        {
            shipEditor.ResourceStorage.S_AddResourceCount(NetCurrencyType.Gold,
                _lobbyConductor.S_GetResourcePerRound());
        }
    }
}
