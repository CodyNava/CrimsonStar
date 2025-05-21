using TMPro;
using UnityEngine;

public class PlayerStatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText, dmgDealt, dmgReceived, roundsWon, status;
    public void SetRoundStats(NetPlayerMatchStats playerStats)
    {
        playerNameText.text = playerStats.player.playerDisplayName;
        dmgDealt.text = playerStats.damageDealtRound.ToString("F0");
        dmgReceived.text = playerStats.damageReceivedRound.ToString("F0");
        roundsWon.text = playerStats.score.ToString("F0");
        status.text = playerStats.wasAlive ? "Survived" : "Defeated";
    }

    public void SetMatchStats(NetPlayerMatchStats playerStats)
    {
        playerNameText.text = playerStats.player.playerDisplayName;
        dmgDealt.text = playerStats.damageDealtMatch.ToString("F0");
        dmgReceived.text = playerStats.damageReceivedMatch.ToString("F0");
        roundsWon.text = playerStats.score.ToString("F0");
        status.text = playerStats.wasAlive ? "Survived" : "Defeated";
    }
}
