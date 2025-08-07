using Steamworks;
using UnityEngine;

public static class PlayerData
{
    public static CSteamID CurrentLobbyID => new(_lobbySteamID);
    public static ulong PlayerID { get; private set; }
    public static string DisplayName { get; private set; }
    public static bool IsLobbyHost { get; private set; }
    
    private static ulong _lobbySteamID;
    public static bool IsPrivateSession { get; private set; }

    public static void SetLobbyID(ulong lobbyId) => _lobbySteamID = lobbyId;

    public static void SetLobbyHost(bool isHost) => IsLobbyHost = isHost;
    
    public static void SetPlayerIDFromSteam(CSteamID userID) => PlayerID = userID.m_SteamID;
    public static void SetPlayerIDFromRandom() => PlayerID = (ulong)(Random.value * (ulong.MaxValue - 2)) + 1;

    public static void SetDisplayName(string displayName) => DisplayName = displayName;

    public static void SetIsPrivateSession(bool isPrivateSession) => IsPrivateSession = isPrivateSession;
}
