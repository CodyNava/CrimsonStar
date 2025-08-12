using System.Collections;
using FishNet;
using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log(netTeamID);
        StartCoroutine(DisableButton());
    }


    private IEnumerator DisableButton()
    {
        var switchTeamsButton = gameObject.GetComponentInChildren<Button>();
        switchTeamsButton.interactable = false;
        yield return new WaitForSeconds(2f);
        switchTeamsButton.interactable = true;
        yield return null;
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