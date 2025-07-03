using FishNet;
using UnityEngine;

public class SwitchTeamsButton : MonoBehaviour
{
    private NetTeamID _teamID;
    private ulong _playerID;

    public void OnButtonClick()
    {
        var netTeamID = (int)_teamID;
        netTeamID++;
        netTeamID %= 2;
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.PlayerTeamChangeRequested
        {
            NewTeamID = (NetTeamID)netTeamID, PlayerID = _playerID
        });
    }

    public void UpdateTeamID(NetTeamID teamID)
    {
        _teamID = teamID;
    }

    public void SetPlayerID(ulong playerID)
    {
        _playerID = playerID;
    }
}
