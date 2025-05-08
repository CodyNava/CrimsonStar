using FishNet.Broadcast;
using Steamworks;

public static class NetLobbyBroadcasts
{
    public struct PlayerIdentified : IBroadcast
    {
        public CSteamID SteamID;
        public string DisplayName;
    }

    public struct PlayerListUpdate : IBroadcast
    {
        public LobbyPlayerData[] Players;
    }

    public struct GameStartRequested : IBroadcast
    {
        
    }
}
