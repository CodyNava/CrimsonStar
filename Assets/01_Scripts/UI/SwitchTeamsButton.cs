using FishNet;
using UnityEngine;

public class SwitchTeamsButton : MonoBehaviour
{
    private NetTeamID _teamID;
    private ulong _playerID;
    

    public void OnButtonClick()
    {
        NetTeamID netTeamID = (int)_teamID <= 1 ? NetTeamID.Team2 : NetTeamID.Team1;
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.PlayerTeamChangeRequested
        {
            NewTeamID = netTeamID, PlayerID = _playerID
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
