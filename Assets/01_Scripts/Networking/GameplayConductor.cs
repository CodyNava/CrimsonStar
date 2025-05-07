using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            _editorConductor = InstanceFinder.GetInstance<ShipEditorConductor>();
            SceneManager.OnClientPresenceChangeStart += OnSceneChange;
        }
    }

    public override void OnStopNetwork()
    {
        InstanceFinder.UnregisterInstance<GameplayConductor>();
        
        if (IsServerInitialized)
        {
            SceneManager.OnClientPresenceChangeStart -= OnSceneChange;
        }
    }

    private void OnSceneChange(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene.name == "NetGameplayScene" && args.Added)
        {
            var bridge = Instantiate(bridgePrefab);
            var spawnPoint = GetSpawnTransform();
            ConstructPlayerShip(args.Connection, bridge, _editorConductor.PlayerShipEditors[args.Connection], args.Scene);
            ServerManager.Spawn(bridge.gameObject, args.Connection, args.Scene);
            bridge.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            if (_spawnedPlayers == PlayerCount)
            {
                StartMatch();
            }
        }
    }

    private void StartMatch()
    {
        Debug.Log("Imagine the game would start right now! Wow!");
    }

    private void ConstructPlayerShip(NetworkConnection conn, NetworkedBridge bridge, ServerShipEditorData editorData, Scene scene)
    {
        HashSet<HexCoordinate> spawnedRoots = new HashSet<HexCoordinate>();
        foreach (var placementData in editorData.ModuleStorage.ModuleMap.Values)
        {
            if (!spawnedRoots.Add(placementData.RootCoordinate)) continue;
            if (placementData.ModuleID <= NetModuleID.Bridge) continue;
            Quaternion moduleRotation = Quaternion.AngleAxis(placementData.Rotation * 60, Vector3.back);
            Vector3 modulePos = bridge.HexTransform.Layout.HexToPositionXY(placementData.RootCoordinate);
            NetGameplayModule module = Instantiate(placementData.ModuleID.GetModuleData().GameplayPrefab);
            module.NetworkObject.SetParent(bridge);
            module.transform.SetLocalPositionAndRotation(bridge.transform.InverseTransformPoint(modulePos), moduleRotation);
            module.LinkToBridge(bridge);
        }
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
