using FishNet.Broadcast;

public static class NetShipEditorBroadcasts
{
    public struct ShipEditorUpdate : IBroadcast
    {
        public int currentRound;
        public int maxRounds;
        public string[] names;
        public bool[] readyState;
    }
    
}