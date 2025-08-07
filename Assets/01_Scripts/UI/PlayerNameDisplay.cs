using System;
using LiteNetLib;
using TMPro;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private NetBridge bridge;
    [SerializeField] private NetGameplayModule bridgeModule;
    

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
                nameDisplay.color = Color.blue;
                break;
            case NetTeamID.Team2:
                nameDisplay.color = Color.red;
                break;
            case NetTeamID.Team3:
                nameDisplay.color = Color.cyan;
                break;
            case NetTeamID.Team4:
                nameDisplay.color = Color.green;
                break;
            case NetTeamID.Team5:
                nameDisplay.color = Color.yellow;
                break;
            case NetTeamID.Team6:
                nameDisplay.color = Color.magenta;
                break;
            case NetTeamID.Team7:
                nameDisplay.color = Color.white;
                break;
            case NetTeamID.Team8:
                nameDisplay.color = Color.grey;
                break;
            
        }
        //nameDisplay.color = inTeamOne ? Color.green : Color.red;
        string name = bridge.DisplayName;
        nameDisplay.text = $"{name}";
        // todo trim chars at [9]
    }
}