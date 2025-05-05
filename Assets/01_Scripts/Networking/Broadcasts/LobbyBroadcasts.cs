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
        
    }
}
