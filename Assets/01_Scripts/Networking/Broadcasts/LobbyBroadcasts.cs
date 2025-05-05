using FishNet.Broadcast;
using Steamworks;

public static class LobbyBroadcasts
{
    public struct PlayerJoined : IBroadcast
    {
        public CSteamID SteamID;
        public string DisplayName;
    }

    public struct PlayerLeft : IBroadcast
    {
        public CSteamID SteamID;
    }

    public struct PlayerListUpdate : IBroadcast
    {
        public LobbyPlayerData[] Players;
    }

    public struct GameStartRequested : IBroadcast
    {
        
    }
}
