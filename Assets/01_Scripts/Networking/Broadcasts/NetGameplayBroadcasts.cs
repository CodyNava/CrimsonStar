using FishNet.Broadcast;
using FishNet.Connection;

public static class NetGameplayBroadcasts
{
    public struct RoundResult : IBroadcast { }
    public struct MatchResult : IBroadcast { }

    public struct PlayerDeath : IBroadcast
    {
        public NetworkConnection conn;
    }
}
