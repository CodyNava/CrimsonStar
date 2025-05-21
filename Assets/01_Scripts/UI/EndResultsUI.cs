using FishNet;
using FishNet.Transporting;
using UnityEngine;

public class EndResultsUI : MonoBehaviour
{
    [SerializeField] private PlayerStatsDisplay[] statsDisplays;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject container;
    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<NetGameplayBroadcasts.RoundResult>(OnRoundResult);
    }

    private void OnRoundResult(NetGameplayBroadcasts.RoundResult arg1, Channel arg2)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        container.SetActive(true);
        for (var i = 0; i < arg1.Stats.Length; i++)
        {
            var playerStats = arg1.Stats[i];
            statsDisplays[i].SetStats(playerStats);
        }
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<NetGameplayBroadcasts.RoundResult>(OnRoundResult);
    }
}
