using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlateDisplay : MonoBehaviour
{
    //[SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text playerName;
    private NetLobbyData _lobbyData;

    public void UpdateDisplay(NetLobbyData lobbyData, NetTeamModeID teamMode)
    {
        _lobbyData = lobbyData;
        //readyImage.gameObject.SetActive(lobbyData.isReady);
        playerName.text = lobbyData.playerDisplayName;
        bool canEdit = lobbyData.playerID == PlayerData.PlayerID || PlayerData.IsLobbyHost;
        /*if (canEdit)
        {
            teamDropDown.gameObject.SetActive(teamMode == NetTeamModeID.TeamMode);
            teamDropDown.SetValueWithoutNotify((int)lobbyData.playerTeamID);
            teamLabel.gameObject.SetActive(false);
        }
        else
        {
            teamDropDown.gameObject.SetActive(false);
            teamLabel.gameObject.SetActive(teamMode == NetTeamModeID.TeamMode);
            teamLabel.text = lobbyData.playerTeamID.ToString();
        }*/
    }

    public void SetTeam(int teamIDint)
    {
        NetTeamID teamID = (NetTeamID)teamIDint;
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.PlayerTeamChangeRequested
        {
            NewTeamID = teamID, PlayerID = _lobbyData.playerID
        });
    }

    public void ResetDisplay()
    {
        //readyImage.gameObject.SetActive(false);
        playerName.text = "-";
    }
}
