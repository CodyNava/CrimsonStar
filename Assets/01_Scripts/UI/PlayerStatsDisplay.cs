using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText, dmgDealt, dmgReceived, frags, roundsWon, status;
    [SerializeField] private Sprite survived, defeated;
    [SerializeField] private Image statusImage;
    public void SetRoundStats(NetMatchPlayer player)
    {
        playerNameText.text = player.DisplayName.Value;
        roundsWon.text = player.MatchScore.Value.ToString("F0");
        frags.text = player.KillsRound.Value.ToString("F0");
        dmgDealt.text = player.DamageDealtRound.Value.ToString("F0");
        statusImage.gameObject.SetActive(true);
        statusImage.sprite = player.Survived.Value ? survived: defeated;
        dmgReceived.text = player.DamageReceivedRound.Value.ToString("F0");
    }

    public void SetMatchStats(NetMatchPlayer player)
    {
        playerNameText.text = player.DisplayName.Value;
        roundsWon.text = player.MatchScore.Value.ToString("F0");
        frags.text = player.KillsMatch.Value.ToString("F0");
        dmgDealt.text = player.DamageDealtMatch.Value.ToString("F0");
        statusImage.gameObject.SetActive(true);
        statusImage.sprite = player.Survived.Value ? survived : defeated;
        dmgReceived.text = player.DamageReceivedMatch.Value.ToString("F0");
    }

    public void TurnOffStats()
    {
        statusImage.gameObject.SetActive(false);
    }
}