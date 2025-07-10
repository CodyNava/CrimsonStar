using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndResultsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private PlayerStatsDisplay[] statsDisplays;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject container;
    [SerializeField] private Button backToMainButton;
    
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

        List<NetMatchPlayer> players = FindObjectsByType<NetMatchPlayer>(FindObjectsSortMode.None).ToList();
        players.Sort((a, b) => a.MatchScore.Value.CompareTo(b.MatchScore.Value));
        for (int i = 0; i < players.Count; i++)
        {
            statsDisplays[i].SetMatchStats(players[i]);
        }
        backToMainButton.gameObject.SetActive(true);
    }

    private void OnRoundResult(NetGameplayBroadcasts.RoundResult msg, Channel channel)
    {
        headerText.text = "Round Result";
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        container.SetActive(true);
        
        List<NetMatchPlayer> players = FindObjectsByType<NetMatchPlayer>(FindObjectsSortMode.None).ToList();
        players.Sort((a, b) => a.MatchScore.Value.CompareTo(b.MatchScore.Value));
        for (int i = 0; i < players.Count; i++)
        {
            statsDisplays[i].SetRoundStats(players[i]);
        }
    }

    public void OnBackToMainPressed()
    {
        NetGameBootstrapper.LeaveLobby();
        SceneManager.LoadScene("00_Scenes/MainMenu");
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
