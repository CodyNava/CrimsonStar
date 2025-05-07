using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class GameplayConductor : NetworkSingleton<GameplayConductor>
{
    [SerializeField] private NetworkedBridge bridgePrefab;
    [SerializeField, SerializedDictionary] 
    private SerializedDictionary<int, Transform[]> spawnTransforms;

    private ShipEditorConductor _editorConductor;

    private int PlayerCount => _editorConductor.PlayerShipEditors.Count;
    private int _spawnedPlayers = 0;
    
    public override void OnStartNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.RegisterInstance(this);
            _editorConductor = InstanceFinder.GetInstance<ShipEditorConductor>();
            SceneManager.OnClientPresenceChangeStart += OnSceneChange;
        }
    }

    public override void OnStopNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.UnregisterInstance<GameplayConductor>();
            SceneManager.OnClientPresenceChangeStart -= OnSceneChange;
        }
    }

    private void OnSceneChange(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene.name == "NetGameplayScene" && args.Added)
        {
            var bridge = Instantiate(bridgePrefab);
            var spawnPoint = GetSpawnTransform();
            bridge.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            ConstructPlayerShip(bridge, _editorConductor.PlayerShipEditors[args.Connection]);
            ServerManager.Spawn(bridge.gameObject, args.Connection, args.Scene);
        }
    }

    private void ConstructPlayerShip(NetworkedBridge bridge, ServerShipEditorData editorData)
    {
        
    }

    private Transform GetSpawnTransform()
    {
        if (!spawnTransforms.TryGetValue(PlayerCount, out Transform[] spawnPoints))
        {
            return transform;
        }

        return spawnPoints[_spawnedPlayers++];
    }
}
