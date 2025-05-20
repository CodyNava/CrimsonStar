using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlateDisplay : MonoBehaviour
{
    [SerializeField] private Image readyImage;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Dropdown teamDropDown;
    [SerializeField] private TMP_Text teamLabel;
    private NetPlayerData _playerData;

    public void UpdateDisplay(NetPlayerData playerData)
    {
        _playerData = playerData;
        readyImage.gameObject.SetActive(playerData.isReady);
        playerName.text = playerData.playerDisplayName;
        bool canEdit = playerData.playerSteamID == SteamPlayer.SteamID || SteamPlayer.IsLobbyHost;
        if (canEdit)
        {
            teamDropDown.gameObject.SetActive(true);
            teamDropDown.SetValueWithoutNotify((int)playerData.playerTeamID);
            teamLabel.gameObject.SetActive(false);
        }
        else
        {
            teamDropDown.gameObject.SetActive(false);
            teamLabel.gameObject.SetActive(true);
            teamLabel.text = playerData.playerTeamID.ToString();
        }
    }

    public void SetTeam(int teamIDint)
    {
        NetTeamID teamID = (NetTeamID)teamIDint;
        InstanceFinder.ClientManager.Broadcast(new NetLobbyBroadcasts.PlayerTeamChangeRequested
        {
            NewTeamID = teamID, Player = _playerData.playerSteamID
        });
    }

    public void ResetDisplay()
    {
        readyImage.gameObject.SetActive(false);
        playerName.text = "-";
        teamDropDown.gameObject.SetActive(false);
        teamLabel.gameObject.SetActive(false);
    }
}
