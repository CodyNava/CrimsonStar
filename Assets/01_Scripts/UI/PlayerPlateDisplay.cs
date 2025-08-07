using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlateDisplay : MonoBehaviour
{
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite unreadySprite;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text playerName;
    private NetLobbyData _lobbyData;

    public void UpdateDisplay(NetLobbyData lobbyData, NetTeamModeID teamMode)
    {
        _lobbyData = lobbyData;
        //readyImage.gameObject.SetActive(lobbyData.isReady);
        background.sprite = lobbyData.isReady ? readySprite : unreadySprite;
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
        background.sprite = unreadySprite;
        playerName.text = "-";
    }
}
