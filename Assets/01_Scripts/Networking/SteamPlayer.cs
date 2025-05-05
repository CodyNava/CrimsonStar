using Steamworks;

public static class SteamPlayer
{
    public static CSteamID CurrentLobbyID => new(_lobbySteamID);
    public static CSteamID SteamID { get; private set; }
    public static string DisplayName { get; private set; }
    
    private static ulong _lobbySteamID;

    public static void SetLobbyID(ulong lobbyId) => _lobbySteamID = lobbyId;

    public static void SetUserID(CSteamID userID) => SteamID = userID;

    public static void SetDisplayName(string displayName) => DisplayName = displayName;
}
