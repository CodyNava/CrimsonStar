using GameKit.Dependencies.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText, dmgDealt, dmgReceived, frags, status, rank;
    [SerializeField] private Sprite survived, defeated;
    [SerializeField] private Image statusImage, roundImage, matchImage;
    [SerializeField] private GameObject roundsContainer;
    [SerializeField] private GameObject roundsStarPrefab;
    public void SetRoundStats(NetMatchPlayer player, int rankNumber)
    {
        playerNameText.text = player.DisplayName.Value; frags.text = player.KillsRound.Value.ToString("F0");
        dmgDealt.text = player.DamageDealtRound.Value.ToString("F0");
        statusImage.gameObject.SetActive(true);
        matchImage.gameObject.SetActive(false);
        roundImage.gameObject.SetActive(player.RoundWon.Value);
        statusImage.sprite = player.Survived.Value ? survived: defeated;
        dmgReceived.text = player.DamageReceivedRound.Value.ToString("F0");
        rank.text = rankNumber.ToString();
        SetUpRoundIndicator(player);
    }

    public void SetMatchStats(NetMatchPlayer player, int rankNumber)
    {
        playerNameText.text = player.DisplayName.Value;
        frags.text = player.KillsMatch.Value.ToString("F0");
        dmgDealt.text = player.DamageDealtMatch.Value.ToString("F0");
        statusImage.gameObject.SetActive(true);
        roundImage.gameObject.SetActive(player.RoundWon.Value);
        matchImage.gameObject.SetActive(player.MatchWon.Value);
        statusImage.sprite = player.Survived.Value ? survived : defeated;
        dmgReceived.text = player.DamageReceivedMatch.Value.ToString("F0");
        rank.text = rankNumber.ToString();
        SetUpRoundIndicator(player);
    }

    private void SetUpRoundIndicator(NetMatchPlayer player)
    {
        roundsContainer.transform.DestroyChildren();
        for (int i = 0; i < player.MatchScore.Value; i++)
        {
           Instantiate(roundsStarPrefab, roundsContainer.transform);
        }
    }
    public void TurnOffStats()
    {
        statusImage.gameObject.SetActive(false);
        matchImage.gameObject.SetActive(false);
        roundImage.gameObject.SetActive(false);
    }
}