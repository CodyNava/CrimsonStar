using System;
using TMPro;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private NetBridge bridge;

    public void Update()
    {
        DisplayName();
    }

    public void DisplayName()
    {
        string name = bridge.DisplayName;
        nameDisplay.text = $"{name}";
    }
}