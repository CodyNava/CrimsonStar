using FishNet.Broadcast;

public static class NetGameplayBroadcasts
{
    public struct RoundResult : IBroadcast { }
    public struct MatchResult : IBroadcast { }
}
