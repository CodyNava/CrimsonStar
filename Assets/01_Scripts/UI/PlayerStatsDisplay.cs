using TMPro;
using UnityEngine;

public class PlayerStatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText, dmgDealt, dmgReceived, frags, roundsWon, status;
    public void SetRoundStats(NetMatchPlayer player)
    {
        playerNameText.text = player.DisplayName.Value;
        dmgDealt.text = player.DamageDealtRound.Value.ToString("F0");
        dmgReceived.text = player.DamageReceivedRound.Value.ToString("F0");
        frags.text = player.KillsRound.Value.ToString("F0");
        roundsWon.text = player.MatchScore.Value.ToString("F0");
        status.text = player.Survived.Value ? "Survived" : "Defeated";
    }

    public void SetMatchStats(NetMatchPlayer player)
    {
        playerNameText.text = player.DisplayName.Value;
        dmgDealt.text = player.DamageDealtMatch.Value.ToString("F0");
        dmgReceived.text = player.DamageReceivedMatch.Value.ToString("F0");
        frags.text = player.KillsMatch.Value.ToString("F0");
        roundsWon.text = player.MatchScore.Value.ToString("F0");
        status.text = player.Survived.Value ? "Survived" : "Defeated";
    }
}
