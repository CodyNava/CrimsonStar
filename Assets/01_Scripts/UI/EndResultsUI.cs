using System.Linq;
using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class EndResultsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private PlayerStatsDisplay[] statsDisplays;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject container;
    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<NetGameplayBroadcasts.RoundResult>(OnRoundResult);
        InstanceFinder.ClientManager.RegisterBroadcast<NetGameplayBroadcasts.MatchResult>(OnMatchResult);
    }

    private void OnMatchResult(NetGameplayBroadcasts.MatchResult msg, Channel channel)
    {
        headerText.text = "Match Result";
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        container.SetActive(true);
        for (var i = 0; i < msg.Stats.Length; i++)
        {
            var playerStats = msg.Stats[i];
            statsDisplays[i].SetMatchStats(playerStats);
        }
    }

    private void OnRoundResult(NetGameplayBroadcasts.RoundResult msg, Channel channel)
    {
        headerText.text = "Round Result";
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        container.SetActive(true);
        for (var i = 0; i < msg.Stats.Length; i++)
        {
            var playerStats = msg.Stats[i];
            statsDisplays[i].SetRoundStats(playerStats);
        }
    }

    private void OnDisable()
    {
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager.UnregisterBroadcast<NetGameplayBroadcasts.RoundResult>(OnRoundResult);
            InstanceFinder.ClientManager.UnregisterBroadcast<NetGameplayBroadcasts.MatchResult>(OnMatchResult);
        }
    }
}
