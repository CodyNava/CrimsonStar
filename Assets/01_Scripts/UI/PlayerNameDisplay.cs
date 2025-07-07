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
        bool inTeamOne = bridgeModule.NetTeamID == NetTeamID.Team1;
        nameDisplay.color = inTeamOne ? Color.green : Color.red;
        string name = bridge.DisplayName;
        nameDisplay.text = $"{name}";
    }
}