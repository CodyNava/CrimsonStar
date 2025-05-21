using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NetPlayerMatchStats
{
    public NetPlayerData player;
    public float damageReceivedRound;
    public float damageReceivedMatch;
    public float damageDealtRound;
    public float damageDealtMatch;
    public int score;
}

public class NetGameplayConductor : NetworkSingleton<NetGameplayConductor>
{
    [SerializeField] private NetBridge bridgePrefab;
    [SerializeField] private float endOfRoundTime;
    
    [SerializeField, SerializedDictionary] 
    private SerializedDictionary<int, Transform[]> spawnTransforms;

    private NetLobbyConductor _lobbyConductor;
    private NetShipEditorConductor _editorConductor;
    private List<NetworkConnection> _eliminatedPlayers = new();
    private Dictionary<NetworkConnection, NetBridge> _bridges = new();
    private Dictionary<NetworkConnection, NetPlayerMatchStats> _matchStats = new();
    
    private readonly SyncVar<bool> _isMatchConcluded = new();
    private readonly SyncDictionary<NetTeamID, int> _scoreBoard = new();
    
    public bool IsMatchConcluded => _isMatchConcluded.Value;
    public IEnumerable<KeyValuePair<NetTeamID, int>> GetScoreCounts() => _scoreBoard;

    private int _roundsPlayed;

    private int PlayerCount => _editorConductor.PlayerShipEditors.Count;
    private int _spawnedPlayers = 0;
    
    public override void OnStartNetwork()
    {
        InstanceFinder.RegisterInstance(this);
        
        if (IsServerInitialized)
        {
            _lobbyConductor = InstanceFinder.GetInstance<NetLobbyConductor>();
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
            NetPlayerData playerData = _lobbyConductor.S_GetPlayerData(args.Connection);

            if (!_matchStats.TryGetValue(args.Connection, out var stats))
            {
                stats = new NetPlayerMatchStats
                {
                    player = playerData
                };
                _matchStats[args.Connection] = stats;
            }

            stats.damageReceivedRound = 0;
            stats.damageDealtRound = 0;
            
            var bridge = Instantiate(bridgePrefab);
            _bridges.Add(args.Connection, bridge);
            var spawnPoint = S_GetSpawnTransform();
            bridge.S_SetDisplayName(playerData.playerDisplayName);
            bridge.GetComponent<NetGameplayModule>().S_ServerInit(bridge, playerData.playerTeamID, HexCoordinate.Zero);
            S_ConstructPlayerShip(args.Connection, playerData.playerTeamID, bridge, _editorConductor.PlayerShipEditors[args.Connection], args.Scene);
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
        C_OnMatchStart();
    }

    [ObserversRpc]
    public void C_OnMatchStart()
    {
        InputManager.EnableGameControls();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void S_RegisterPlayerDeath(Channel channel = Channel.Reliable, NetworkConnection conn = null)
    {
        _bridges.Remove(conn!);
        _eliminatedPlayers.Add(conn);
        if (S_IsMatchComplete())
        {
            S_StopMatch();
        }
    }

    private bool S_IsMatchComplete()
    {
        NetTeamID teamID = NetTeamID.Observer;
        
        foreach (var (conn, playData) in _lobbyConductor.ConnectionPlayerMap)
        {
            if (_eliminatedPlayers.Contains(conn)) continue;

            if (teamID == NetTeamID.Observer)
            {
                teamID = playData.playerTeamID;
            }
            else
            {
                if (teamID != playData.playerTeamID) return false;
            }
        }

        int score = _scoreBoard.GetValueOrDefault(teamID) + 1;
        _scoreBoard[teamID] = score;

        foreach (var stats in _matchStats.Values)
        {
            if (stats.player.playerTeamID == teamID)
            {
                stats.score = score;
            }
        }
        ServerManager.Broadcast(new NetGameplayBroadcasts.RoundResult
        {
            Stats = _matchStats.Values.ToArray()
        });
        return true;
    }

    private void S_StopMatch()
    {
        _roundsPlayed++;
        if (_roundsPlayed >= _lobbyConductor.S_GetRoundCount())
        {
            StartCoroutine(EndOfMatchRoutine());
        }
        else
        {
            _isMatchConcluded.Value = true;
            StartCoroutine(EndOfRoundRoutine());
        }
    }

    private IEnumerator EndOfRoundRoutine()
    {
        foreach (var (_, bridge) in _bridges)
        {
            bridge.HandleEndOfRound();
        }
        yield return new WaitForSecondsRealtime(endOfRoundTime);
        _spawnedPlayers = 0;
        _editorConductor.S_SetupNewEditPhase();
        SceneLoadData sceneData = new("NetShipEditor");
        sceneData.PreferredActiveScene = new PreferredScene(sceneData.SceneLookupDatas[0]);
        sceneData.MovedNetworkObjects =
            _editorConductor.PlayerShipEditors.Values.Select(data => data.NetworkObject).ToArray();
        SceneUnloadData unloadData = new("NetGameplayScene");
        SceneManager.LoadGlobalScenes(sceneData);
        SceneManager.UnloadGlobalScenes(unloadData);
        _isMatchConcluded.Value = false;
    }

    private IEnumerator EndOfMatchRoutine()
    {
        yield return new WaitForSecondsRealtime(endOfRoundTime);
        _spawnedPlayers = 0;
        SceneLoadData sceneData = new("NetLobby");
        sceneData.PreferredActiveScene = new PreferredScene(sceneData.SceneLookupDatas[0]);
        SceneUnloadData unloadData = new("NetGameplayScene");
        SceneManager.LoadGlobalScenes(sceneData);
        SceneManager.UnloadGlobalScenes(unloadData);
        _isMatchConcluded.Value = false;
    }

    private void S_ConstructPlayerShip(NetworkConnection conn, NetTeamID id, NetBridge bridge, NetShipEditorData editorData, Scene scene)
    {
        foreach (var placementData in editorData.ModuleStorage.GetUniqueModules())
        {
            Quaternion moduleRotation = Quaternion.AngleAxis(placementData.Rotation * 60, Vector3.back);
            Vector3 modulePos = bridge.HexTransform.Layout.HexToPositionXY(placementData.RootCoordinate);
            NetGameplayModule module = Instantiate(placementData.ModuleID.GetModuleData().GameplayPrefab);
            module.NetworkObject.SetParent(bridge);
            module.transform.SetLocalPositionAndRotation(bridge.transform.InverseTransformPoint(modulePos), moduleRotation);
            module.S_ServerInit(bridge, id, placementData.RootCoordinate);
        }
    }
    
    public void S_ReportDamageInstance(CSteamID attacker, NetworkConnection defender, float damageTaken)
    {
        var attackerStats = _matchStats.First(x => x.Value.player.playerSteamID == attacker).Value;
        attackerStats.damageDealtRound += damageTaken;
        attackerStats.damageDealtMatch += damageTaken;
        var defenderStats = _matchStats[defender];
        defenderStats.damageReceivedRound += damageTaken;
        defenderStats.damageReceivedMatch += damageTaken;
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
