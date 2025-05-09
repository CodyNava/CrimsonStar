using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetGameplayConductor : NetworkSingleton<NetGameplayConductor>
{
    [SerializeField] private NetBridge bridgePrefab;
    [SerializeField, SerializedDictionary] 
    private SerializedDictionary<int, Transform[]> spawnTransforms;

    private NetShipEditorConductor _editorConductor;

    private int PlayerCount => _editorConductor.PlayerShipEditors.Count;
    private int _spawnedPlayers = 0;
    
    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            _editorConductor = InstanceFinder.GetInstance<NetShipEditorConductor>();
            SceneManager.OnClientPresenceChangeStart += S_OnSceneChange;
        }
    }

    public override void OnStopNetwork()
    {
        InstanceFinder.UnregisterInstance<NetGameplayConductor>();
        
        if (IsServerInitialized)
        {
            SceneManager.OnClientPresenceChangeStart -= S_OnSceneChange;
        }
    }

    private void S_OnSceneChange(ClientPresenceChangeEventArgs args)
    {
        if (args.Scene.name == "NetGameplayScene" && args.Added)
        {
            NetPlayerID id = (NetPlayerID) _spawnedPlayers + 1;
            var bridge = Instantiate(bridgePrefab);
            var spawnPoint = S_GetSpawnTransform();
            bridge.GetComponent<NetGameplayModule>().S_ServerInit(bridge, id);
            S_ConstructPlayerShip(args.Connection, id, bridge, _editorConductor.PlayerShipEditors[args.Connection], args.Scene);
            ServerManager.Spawn(bridge.gameObject, args.Connection);
            bridge.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            if (_spawnedPlayers == PlayerCount)
            {
                S_StartMatch();
            }
        }
    }

    private void S_StartMatch()
    {
        Debug.Log("Imagine the game would start right now! Wow!");
    }

    private void S_ConstructPlayerShip(NetworkConnection conn, NetPlayerID id, NetBridge bridge, NetShipEditorData editorData, Scene scene)
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
            module.S_ServerInit(bridge, id);
        }
    }

    private Transform S_GetSpawnTransform()
    {
        if (!spawnTransforms.TryGetValue(PlayerCount, out Transform[] spawnPoints))
        {
            return transform;
        }

        return spawnPoints[_spawnedPlayers++];
    }
}
