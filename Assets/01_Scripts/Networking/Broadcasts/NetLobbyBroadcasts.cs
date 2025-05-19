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
    }

    public struct PlayerTeamChangeRequested : IBroadcast
    {
        public CSteamID Player;
        public NetTeamID NewTeamID;
    }

    public struct SetLobbySettings : IBroadcast
    {
        public int NumberOfRounds;
        public int ResourceGainPerRound;
        public bool CanRecycleModules;
    }

    public struct SetReadyState : IBroadcast
    {
        public bool ReadyState;
    }

    public struct GameStartRequested : IBroadcast
    {
        
    }
}
