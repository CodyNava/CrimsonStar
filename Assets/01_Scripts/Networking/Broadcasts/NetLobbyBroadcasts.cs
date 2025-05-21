using FishNet.Broadcast;
using Steamworks;

public static class NetLobbyBroadcasts
{
    public struct PlayerIdentified : IBroadcast
    {
        public CSteamID SteamID;
        public string DisplayName;
        public bool IsHost;
    }

    public struct PlayerListUpdate : IBroadcast
    {
        public NetPlayerData[] Players;
        public NetTeamModeID TeamMode;
    }

    public struct PlayerTeamChangeRequested : IBroadcast
    {
        public CSteamID Player;
        public NetTeamID NewTeamID;
    }

    public struct SetGameMode : IBroadcast
    {
        public NetGameModeID GameMode;
    }

    public struct SetTeamMode : IBroadcast
    {
        public NetTeamModeID TeamMode;
    }

    public struct SetReadyState : IBroadcast
    {
        public bool ReadyState;
    }

    public struct GameStartRequested : IBroadcast
    {
        
    }
}
