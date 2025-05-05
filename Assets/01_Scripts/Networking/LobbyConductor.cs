using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

[System.Serializable]
public struct LobbyPlayerData
{
    public CSteamID playerSteamID;
    public string playerDisplayName;
}

public class LobbyConductor : NetworkSingleton<LobbyConductor>
{
    private readonly List<LobbyPlayerData> _playerData = new();
    private readonly Dictionary<NetworkConnection, LobbyPlayerData> _connectionPlayerMap = new();

    public override void OnStartNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.RegisterInstance(this);
            ServerManager.RegisterBroadcast<LobbyBroadcasts.PlayerJoined>(OnPlayerJoined, false);
            ServerManager.RegisterBroadcast<LobbyBroadcasts.PlayerLeft>(OnPlayerLeft, false);
            ServerManager.RegisterBroadcast<LobbyBroadcasts.GameStartRequested>(OnGameStartRequested, false);
        }
    }

    private void OnGameStartRequested(NetworkConnection connection, LobbyBroadcasts.GameStartRequested msg, Channel channel)
    {
        Debug.Log("Game start requested");
    }

    private void OnPlayerJoined(NetworkConnection connection, LobbyBroadcasts.PlayerJoined msg, Channel channel)
    {
        int idx = _playerData.FindIndex(player => player.playerSteamID == msg.SteamID);
        if (idx != -1)
            return;
        var data = new LobbyPlayerData
        {
            playerSteamID = msg.SteamID,
            playerDisplayName = msg.DisplayName,
        };
        _playerData.Add(data);
        _connectionPlayerMap[connection] = data;
        ServerManager.Broadcast(new LobbyBroadcasts.PlayerListUpdate
        {
            Players = _playerData.ToArray()
        });
    }

    private void OnPlayerLeft(NetworkConnection connection, LobbyBroadcasts.PlayerLeft msg, Channel channel)
    {
        int idx = _playerData.FindIndex(player => player.playerSteamID == msg.SteamID);
        if (idx == -1)
            return;
        
        _playerData.RemoveAt(idx);
        _connectionPlayerMap.Remove(connection);
        ServerManager.Broadcast(new LobbyBroadcasts.PlayerListUpdate
        {
            Players = _playerData.ToArray()
        });
    }

    public override void OnStopNetwork()
    {
        if (IsServerInitialized)
        {
            InstanceFinder.UnregisterInstance<LobbyConductor>();
            ServerManager.UnregisterBroadcast<LobbyBroadcasts.PlayerJoined>(OnPlayerJoined);
            ServerManager.UnregisterBroadcast<LobbyBroadcasts.GameStartRequested>(OnGameStartRequested);
        }
    }
}
