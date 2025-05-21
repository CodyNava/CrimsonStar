using FishNet.Broadcast;

public static class NetGameplayBroadcasts
{
    public struct RoundResult : IBroadcast
    {
        public NetPlayerMatchStats[] Stats;
    }

    public struct MatchResult : IBroadcast
    {
        public NetPlayerMatchStats[] Stats;
    }
}
