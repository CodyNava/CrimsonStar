using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using Unity.VisualScripting;
using UnityEngine;

public class SpectatorPlayerPanel : MonoBehaviour
{
    [SerializeField] private SpectatorPlayerPanelEntry _panelEntryPrefab;
    [SerializeField] private GameObject _panelContainer;
    [SerializeField] private CameraFollow _cameraFollow;

    private Dictionary<ulong, SpectatorPlayerPanelEntry> playerPanelEntryMap = new();

    private NetGameplayConductor _gameplayConductor;
    private NetLobbyConductor _lobbyConductor;

    private bool _initialized;

    public void Start()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<NetGameplayBroadcasts.PlayerDeath>(OnPlayerDeath);
    }

    private void OnPlayerDeath(NetGameplayBroadcasts.PlayerDeath msg, Channel channel)
    {
        NetworkConnection conn = msg.conn;
        
        if(conn == InstanceFinder.ClientManager.Connection) _panelContainer.SetActive(true);
        
        NetMatchPlayer player = _lobbyConductor.PlayersByConnection[conn];
        if (player.IsUnityNull()) return;

        SpectatorPlayerPanelEntry panelObject = playerPanelEntryMap[player.PlayerID.Value];
        if (panelObject.IsUnityNull()) return;

        panelObject.SetEnable = false;
    }


    private void Init()
    {
        if (_lobbyConductor.IsUnityNull() && !InstanceFinder.TryGetInstance(out _lobbyConductor)) return;
        List<NetMatchPlayer> players = FindObjectsByType<NetMatchPlayer>(FindObjectsSortMode.None).ToList();
        if (players.Count != _lobbyConductor.PlayersByConnection.Count) return;

        foreach (NetMatchPlayer player in players)
        {
            if (player.BridgeObject.Value.IsUnityNull()) return;
        }
        
        playerPanelEntryMap.Clear();
        foreach (NetMatchPlayer player in players)
        {
            SpectatorPlayerPanelEntry entry = Instantiate(_panelEntryPrefab, _panelContainer.transform, true);
            entry.Init(player.DisplayName.Value, this, player.BridgeObject.Value);
            entry.transform.localScale = Vector3.one;
            entry.SetEnable = player.Survived.Value;
            
            playerPanelEntryMap.Add(player.PlayerID.Value, entry);
        }
        
        _panelContainer.SetActive(false);
        _initialized = true;
    }

    public void Update()
    {
        if (!_initialized) Init();
    }

    public void OnDestroy()
    {
        if(InstanceFinder.ClientManager)
            InstanceFinder.ClientManager.UnregisterBroadcast<NetGameplayBroadcasts.PlayerDeath>(OnPlayerDeath);
    }

    public void OnPlayerPanelClicked(NetBridge bridge)
    {
        if (_cameraFollow.IsUnityNull()) return;
        _cameraFollow.SetTarget(bridge);
    }
}
