using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class NetGameplayConductor : BaseConductor<NetGameplayConductor>
{
    private struct KillInstance
    {
        public ulong AttackerID;
        public ulong DefenderID;
    }

    private struct DamageInstance
    {
        public ulong AttackerID;
        public ulong DefenderID;
        public float DamageTaken;
    }

    [SerializeField] private NetBridge bridgePrefab;
    [SerializeField] private float endOfRoundTime;

    [SerializeField, SerializedDictionary]
    private AYellowpaper.SerializedCollections.SerializedDictionary<int, Transform[]> spawnTransforms;

    private NetLobbyConductor _lobbyConductor;
    private NetShipEditorConductor _editorConductor;
    private List<NetworkConnection> _eliminatedPlayers = new();
    private Dictionary<NetworkConnection, NetBridge> _bridges = new();
    public Dictionary<NetworkConnection, NetBridge> Bridges => _bridges;

    private readonly SyncVar<bool> _isMatchConcluded = new();
    private readonly SyncDictionary<NetTeamID, int> _scoreBoard = new();
    private HashSet<Transform> _spawnSet = new();
    private List<DamageInstance> _damageInstancesRound = new();
    private List<KillInstance> _killInstancesRound = new();

    // public bool IsLocalPlayerAlive => _lobbyConductor.PlayersByConnection[InstanceFinder.ClientManager.Connection].Survived.Value;

    private int _roundsPlayed;

    private int PlayerCount => _lobbyConductor.Players.Length;
    private int _spawnedPlayers = 0;

    public override string ConductedSceneName => "NetGameplayScene";


    public event UnityAction<RegisterPlayerDeathEventArgs> OnRegisterPlayerDeath;
    public event UnityAction<LocalPlayerDeathEventArgs> OnLocalPlayerDeath;


    protected override void OnNetworkStarted()
    {
        StartCoroutine(LoadDependencies());
    }

    private IEnumerator LoadDependencies()
    {
        if (!InstanceFinder.TryGetInstance(out _lobbyConductor))
        {
            yield return null;
        }

        if (!InstanceFinder.TryGetInstance(out _editorConductor))
        {
            yield return null;
        }
    }

    public override void ProcessClientAddition(NetworkConnection connection, Scene scene)
    {
        NetMatchPlayer matchPlayer = _lobbyConductor.PlayersByConnection[connection];

        matchPlayer.S_ResetRoundStats();

        var bridge = Instantiate(bridgePrefab);
        ServerManager.Spawn(bridge.gameObject, connection);
        _bridges.Add(connection, bridge);
        _lobbyConductor.PlayersByConnection[connection].BridgeObject.Value = bridge;
        bridge.S_SetDisplayName(matchPlayer.DisplayName.Value);
        bridge.S_SetPlayerID(matchPlayer.PlayerID.Value);
        bridge.GetComponent<NetGameplayModule>().S_ServerInit(bridge, matchPlayer.Team.Value, HexCoordinate.Zero);
        S_ConstructPlayerShip(connection, matchPlayer.Team.Value, bridge, matchPlayer.ModuleStorage, scene);
        var spawnPoint = S_GetSpawnTransform();
        bridge.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        if (_spawnedPlayers == PlayerCount)
        {
            S_StartMatch();
        }
    }

    private void S_StartMatch()
    {
        C_OnMatchStart();
    }

    [ObserversRpc]
    public void C_OnMatchStart()
    {
        SceneAudioManager.instance.IncreaseMusicProgress();
        InputManager.EnableGameControls();
    }

    [Server]
    public void S_RegisterPlayerDeath(NetworkConnection owner)
    {
        _bridges.Remove(owner);
        _eliminatedPlayers.Add(owner);


        if (_eliminatedPlayers.Count >= PlayerCount * 0.5f)
        {
            SceneAudioManager.instance.IncreaseMusicProgress();
            C_TriggerIncreaseMusicProgress();
        }

        C_TriggerOnRegisterPlayerDeath(owner);
        TriggerOnRegisterPlayerDeath(owner);

        if (S_IsMatchComplete())
        {
            S_StopMatch();
        }
    }

    [Client]
    [ObserversRpc]
    private void C_TriggerOnRegisterPlayerDeath(NetworkConnection conn)
    {
        TriggerOnRegisterPlayerDeath(conn);
    }

    // TODO: Refactor as Broadcast with IBroadcast
    private void TriggerOnRegisterPlayerDeath(NetworkConnection conn)
    {
        if (conn == InstanceFinder.ClientManager.Connection)
        {
            OnLocalPlayerDeath?.Invoke(new LocalPlayerDeathEventArgs());
        }

        OnRegisterPlayerDeath?.Invoke(new RegisterPlayerDeathEventArgs(conn));
    }

    [Server]
    private bool S_IsMatchComplete()
    {
        HashSet<NetTeamID> teamIDs = new HashSet<NetTeamID>();

        foreach (var (conn, _) in _bridges)
        {
            var teamID = _lobbyConductor.PlayersByConnection[conn].Team.Value;
            if (teamID == NetTeamID.Observer) continue;
            teamIDs.Add(teamID);
        }

        if (teamIDs.Count > 1) return false;

        NetTeamID winnerID = teamIDs.First();

        int score = _scoreBoard.GetValueOrDefault(winnerID) + 1;
        _scoreBoard[winnerID] = score;

        foreach (var player in _lobbyConductor.PlayersByConnection.Values)
        {
            player.MatchScore.Value = _scoreBoard.GetValueOrDefault(player.Team.Value);
        }

        return true;
    }

    [Server]
    private void S_StopMatch()
    {
        _roundsPlayed++;

        S_CalculateRoundResults();

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

    private IEnumerator EndOfMatchRoutine()
    {
        foreach (var (conn, bridge) in _bridges)
        {
            _lobbyConductor.PlayersByConnection[conn].Survived.Value = true;
            bridge.HandleEndOfRound();
        }

        _bridges.Clear();
        yield return new WaitForSecondsRealtime(3f);
        ServerManager.Broadcast(new NetGameplayBroadcasts.MatchResult());
        SceneAudioManager.instance.StopInGameMusic();
        SceneAudioManager.instance.ResetMusicProgress();
        C_TriggerStopMusic();
    }

    private IEnumerator EndOfRoundRoutine()
    {
        foreach (var (_, bridge) in _bridges)
        {
            bridge.HandleEndOfRound();
        }

        _bridges.Clear();
        yield return new WaitForSecondsRealtime(3f);
        ServerManager.Broadcast(new NetGameplayBroadcasts.RoundResult());
        yield return new WaitForSecondsRealtime(endOfRoundTime);
        _spawnedPlayers = 0;
        _editorConductor.S_SetupNewEditPhase();
        InstanceFinder.GetInstance<NetShipEditorConductor>().MoveToScene(this, _lobbyConductor.Players);
        _isMatchConcluded.Value = false;
        SceneAudioManager.instance.StopInGameMusic();
        SceneAudioManager.instance.ResetMusicProgress();
        SceneAudioManager.instance.StartInGameMusic();
        C_TriggerResetMusic();
    }


    [ObserversRpc]
    [Client]
    private void C_TriggerIncreaseMusicProgress()
    {
        IncreaseMusicProgress();
    }

    private void IncreaseMusicProgress()
    {
        SceneAudioManager.instance.IncreaseMusicProgress();
    }

    [ObserversRpc]
    [Client]
    private void C_TriggerResetMusic()
    {
        ResetMusic();
    }

    private void ResetMusic()
    {
        SceneAudioManager.instance.StopInGameMusic();
        SceneAudioManager.instance.ResetMusicProgress();
        SceneAudioManager.instance.StartInGameMusic();
    }

    [ObserversRpc]
    [Client]
    private void C_TriggerStopMusic()
    {
        StopMusic();
    }

    private void StopMusic()
    {
        SceneAudioManager.instance.StopInGameMusic();
        SceneAudioManager.instance.ResetMusicProgress();
    }

    [Server]
    private void S_ConstructPlayerShip(NetworkConnection conn, NetTeamID id, NetBridge bridge,
        NetModuleStorage editorData, Scene scene)
    {
        foreach (var placementData in editorData.GetUniqueModules())
        {
            Quaternion moduleRotation = Quaternion.AngleAxis(placementData.Rotation * 60, Vector3.back);
            Vector3 modulePos = bridge.HexTransform.Layout.HexToPositionXY(placementData.RootCoordinate);
            NetGameplayModule module = Instantiate(placementData.ModuleID.GetModuleData().GameplayPrefab);
            module.NetworkObject.SetParent(bridge);
            module.transform.SetLocalPositionAndRotation(bridge.transform.InverseTransformPoint(modulePos),
                moduleRotation);
            ServerManager.Spawn(module.gameObject, conn);
            module.S_ServerInit(bridge, id, placementData.RootCoordinate);
        }
    }

    [Server]
    public void S_ReportDamageInstance(ulong attacker, ulong defender, float damageTaken)
    {
        _damageInstancesRound.Add(new DamageInstance
        {
            AttackerID = attacker,
            DefenderID = defender,
            DamageTaken = damageTaken
        });
    }

    [Server]
    public void S_ReportKillInstance(ulong attackerID, ulong defenderID)
    {
        _lobbyConductor.PlayersByID[defenderID].Survived.Value = false;
        _killInstancesRound.Add(new KillInstance
        {
            AttackerID = attackerID,
            DefenderID = defenderID
        });

        NetworkConnection conn = _lobbyConductor.ConnectionsByPlayerID[attackerID];
        if(conn != null) TriggerKillAnnouncer(conn, Channel.Reliable);
    }

    [TargetRpc]
    private void TriggerKillAnnouncer(NetworkConnection conn, Channel channel)
    {
        // TODO: Trigger Kill Announcer SFX
    }

    [Server]
    private void S_CalculateRoundResults()
    {
        foreach (var damageInstance in _damageInstancesRound)
        {
            var attacker = _lobbyConductor.PlayersByID[damageInstance.AttackerID];
            var defender = _lobbyConductor.PlayersByID[damageInstance.DefenderID];
            attacker.DamageDealtRound.Value += damageInstance.DamageTaken;
            attacker.DamageDealtMatch.Value += damageInstance.DamageTaken;
            defender.DamageReceivedRound.Value += damageInstance.DamageTaken;
            defender.DamageReceivedMatch.Value += damageInstance.DamageTaken;
        }

        foreach (var killInstance in _killInstancesRound)
        {
            var attacker = _lobbyConductor.PlayersByID[killInstance.AttackerID];
            var defender = _lobbyConductor.PlayersByID[killInstance.DefenderID];
            attacker.KillsRound.Value += 1;
            attacker.KillsMatch.Value += 1;
        }

        _damageInstancesRound.Clear();
        _killInstancesRound.Clear();
    }

    [Server]
    private Transform S_GetSpawnTransform()
    {
        if (!spawnTransforms.TryGetValue(PlayerCount, out Transform[] spawnPoints))
        {
            return transform;
        }

        if (_spawnedPlayers == 0)
        {
            _spawnSet.AddRange(spawnPoints);
        }

        int rng = Random.Range(0, _spawnSet.Count);
        var spawn = _spawnSet.ElementAt(rng);
        _spawnSet.Remove(spawn);
        _spawnedPlayers++;
        return spawn;
    }

    public struct RegisterPlayerDeathEventArgs
    {
        private NetworkConnection _owner;
        public NetworkConnection Owner => _owner;

        public RegisterPlayerDeathEventArgs(NetworkConnection owner)
        {
            _owner = owner;
        }
    }

    public struct LocalPlayerDeathEventArgs
    {
    }
}