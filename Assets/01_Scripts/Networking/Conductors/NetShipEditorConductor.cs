using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetShipEditorConductor : BaseConductor<NetShipEditorConductor>
{
    [SerializeField] private int minimumResourceCount;
    [SerializeField] private float shipEditorTimerDuration;
    [SerializeField] private FMODUnity.StudioEventEmitter intro;
    [SerializeField] private bool inEditorPhase = true;

    public override string ConductedSceneName => "NetShipEditor";

    private Dictionary<NetworkConnection, bool> _playersReady = new();
    private readonly SyncTimer _editorTimer = new();

    private NetLobbyConductor _lobbyConductor;

    public float TimeRemaining => _editorTimer.Remaining;


    protected override void OnNetworkStarted()
    {
        C_TriggerSwapMusic();
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
            inEditorPhase = true;
            C_ToggleInEditorBool();
            _editorTimer.StartTimer(_lobbyConductor.EditorTimerDuration);
            _editorTimer.OnChange += OnTimerChange;
        }

        _playersReady.Add(connection, false);
    }

    private void Update()
    {
        if (!inEditorPhase) return;
        _editorTimer.Update(Time.deltaTime);
    }
    

    private void OnTimerChange(SyncTimerOperation op, float prev, float next, bool asServer)
    {
        if (asServer && op == SyncTimerOperation.Finished)
        {
            StartCoroutine(AdvanceToGameplayScene());
            _editorTimer.OnChange -= OnTimerChange;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    [Server]
    public void S_SignalReady(Channel channel = Channel.Reliable, NetworkConnection conn = null)
    {
        _playersReady[conn!] = true;

        if (S_AllPlayersReady())
        {
            StartCoroutine(AdvanceToGameplayScene());
        }
    }

    [ObserversRpc]
    [Client]
    private void C_TriggerSwapMusic()
    {
        SwapMusic();
    }

    private void SwapMusic()
    {
        SceneAudioManager.instance.StopMainMusic();
        SceneAudioManager.instance.StartInGameMusic();
    }


    [ObserversRpc]
    [Client]
    private void C_TriggerIntroSound()
    {
        TriggerIntroSound();
        inEditorPhase = false;
    }

    private void TriggerIntroSound()
    {
        intro.Play();
        inEditorPhase = false;
    }

    [ObserversRpc]
    [Client]
    private void C_ToggleInEditorBool()
    {
        inEditorPhase = true;
    }

    private IEnumerator AdvanceToGameplayScene()
    {
        C_TriggerIntroSound();
        TriggerIntroSound();
        yield return new WaitForSecondsRealtime(6.5f);
        _editorTimer.StartTimer(_lobbyConductor.EditorTimerDuration);
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
        inEditorPhase = true;
        C_ToggleInEditorBool();
        foreach (NetMatchPlayer matchPlayer in _lobbyConductor.PlayersByID.Values)
        {
            matchPlayer.ResourceCount.Value += resourceAdded;
        }
    }
}