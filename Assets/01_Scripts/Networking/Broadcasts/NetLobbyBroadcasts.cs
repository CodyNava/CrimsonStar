using FishNet.Broadcast;

public static class NetLobbyBroadcasts
{
    public struct PlayerIdentified : IBroadcast
    {
        public ulong PlayerID;
        public string DisplayName;
        public bool IsHost;
    }

    public struct PlayerListUpdate : IBroadcast
    {
        public NetLobbyData[] Players;
        public NetTeamModeID TeamMode;
    }

    public struct PlayerTeamChangeRequested : IBroadcast
    {
        public ulong PlayerID;
        public NetTeamID NewTeamID;
    }

    public struct SetGameMode : IBroadcast
    {
        public NetGameModeID GameMode;
        public int BaseCurrency;
        public int CurrencyAddedPerRound;
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
