using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetShipEditorConductor : BaseConductor<NetShipEditorConductor>
{
    [SerializeField] private int minimumResourceCount;
    [SerializeField] private float shipEditorTimerDuration;
    [SerializeField] private FMODUnity.StudioEventEmitter intro;
    public override string ConductedSceneName => "NetShipEditor";

    private Dictionary<NetworkConnection, bool> _playersReady = new();
    private readonly SyncTimer _editorTimer = new();

    private NetLobbyConductor _lobbyConductor;

    public float TimeRemaining => _editorTimer.Remaining;


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
    }

    public override void ProcessClientAddition(NetworkConnection connection, Scene scene)
    {
        if (_playersReady.Count == 0)
        {
            _editorTimer.StartTimer(shipEditorTimerDuration);
            _editorTimer.OnChange += OnTimerChange;
        }

        _playersReady.Add(connection, false);
    }

    private void Update()
    {
        _editorTimer.Update(Time.deltaTime);
    }

    private void OnTimerChange(SyncTimerOperation op, float prev, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished)
        {
            TriggerIntroSound();
            C_TriggerIntroSound();
            _editorTimer.OnChange -= OnTimerChange;
        }
    }

    [ServerRpc(RequireOwnership = false)][Server]
    public void S_SignalReady(Channel channel = Channel.Reliable, NetworkConnection conn = null)
    {
        _playersReady[conn!] = true;

        if (S_AllPlayersReady())
        {
            TriggerIntroSound();
            C_TriggerIntroSound();
        }
    }

    [ObserversRpc][Client]
    private void C_TriggerIntroSound()
    {
        TriggerIntroSound();
    }
    
    private void TriggerIntroSound()
    {
        StartCoroutine(PlayIntroSound());
    }

    public IEnumerator PlayIntroSound()
    {
        intro.Play();
        yield return new WaitForSecondsRealtime(6.5f);
        if(IsServerStarted) AdvanceToGameplayScene();
    }

    private void AdvanceToGameplayScene()
    {
        InstanceFinder.GetInstance<NetGameplayConductor>().MoveToScene(this, _lobbyConductor.Players);
        _playersReady.Clear();
    }

    private bool S_AllPlayersReady()
    {
        return _playersReady.Values.All(ready => ready);
    }

    public void S_SetupNewEditPhase()
    {
        int resourceAdded = _lobbyConductor.S_GetResourcePerRound();

        foreach (NetMatchPlayer matchPlayer in _lobbyConductor.PlayersByID.Values)
        {
            matchPlayer.ResourceCount.Value += resourceAdded;
        }
    }
}