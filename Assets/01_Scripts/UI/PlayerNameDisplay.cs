using System;
using System.Collections.Generic;
using LiteNetLib;
using TMPro;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private NetBridge bridge;
    [SerializeField] private NetGameplayModule bridgeModule;
    [SerializeField] private List<TMP_Text> nameTextSizeText;
    [SerializeField] private List<PlayerNameDisplay> playerNameDisplay;

    public void Start()
    {
        DisplayName();
        playerNameDisplay = new List<PlayerNameDisplay>(FindObjectsByType<PlayerNameDisplay>(FindObjectsSortMode.None));
        foreach (var playerName in playerNameDisplay)
        {
            nameTextSizeText.Add(playerName.nameDisplay);
        }
    }
    public void Update()
    {
        DisplayName();
        gameObject.transform.rotation = Quaternion.identity;
    }

    public void DisplayName()
    {
        //bool inTeamOne = bridgeModule.NetTeamID == NetTeamID.Team1;

        switch (bridgeModule.NetTeamID)
        {
            case NetTeamID.Team1:
                nameDisplay.color = new Color(0f, 0f, 1f, 0.5f); 
                break;
            case NetTeamID.Team2:
                nameDisplay.color = new Color(1f, 0f, 0f, 0.5f); 
                break;
            case NetTeamID.Team3:
                nameDisplay.color = new Color(0f, 1f, 1f, 0.5f); 
                break;
            case NetTeamID.Team4:
                nameDisplay.color = new Color(0f, 1f, 0f, 0.5f); 
                break;
            case NetTeamID.Team5:
                nameDisplay.color = new Color(1f, 0.92f, 0.016f, 0.5f); 
                break;
            case NetTeamID.Team6:
                nameDisplay.color = new Color(1f, 0f, 1f, 0.5f); 
                break;
            case NetTeamID.Team7:
                nameDisplay.color = new Color(1f, 1f, 1f, 0.5f); 
                break;
            case NetTeamID.Team8:
                nameDisplay.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); 
                break;
            
        }
        Vector3 GetClampedScreenPosition(Vector3 worldPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            if (screenPos.z < 0)
            {
                screenPos.x = Screen.width - screenPos.x;
                screenPos.y = Screen.height - screenPos.y;
                screenPos.z = 0;
            }
            screenPos.x = Mathf.Clamp(screenPos.x, 50, Screen.width - 50);
            screenPos.y = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);

            return screenPos;
        }
        
        nameDisplay.text = bridge.DisplayName;
        
        for (int i = 0; i < playerNameDisplay.Count; i++)
        {
            var player = playerNameDisplay[i];
            var textElement = nameTextSizeText[i];

            if (!player) continue;
            textElement.fontSize = (2.5f - playerNameDisplay.Count / 8f) * (bridge.CameraZoom.ZoomDistance / 100);
            
            Vector3 playerWorldPos = player.transform.position + Vector3.up * 10f;
            
            Vector3 clampedScreenPos = GetClampedScreenPosition(playerWorldPos);
            
            float depth = Camera.main.WorldToScreenPoint(playerWorldPos).z;
            Vector3 worldCanvasPos = Camera.main.ScreenToWorldPoint(new Vector3(clampedScreenPos.x, clampedScreenPos.y, depth));

            textElement.transform.position = worldCanvasPos;
        }


        
    }
    
    
}