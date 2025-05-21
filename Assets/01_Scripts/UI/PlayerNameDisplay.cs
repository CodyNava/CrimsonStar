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
        gameObject.transform.rotation = Quaternion.identity;
    }

    public void DisplayName()
    {
        string name = bridge.DisplayName;
        nameDisplay.text = $"{name}";
    }
}