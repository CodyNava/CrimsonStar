using System;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using TMPro;

public class ShipEditorTimerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timerDisplay;

    public void Update()
    {
        DisplayTimer();
    }

    void DisplayTimer()
    {
        if (InstanceFinder.HasInstance<NetShipEditorConductor>())
        {
            float remainingTime = NetShipEditorConductor.Instance.TimeRemaining;
            timerDisplay.text = $"{remainingTime:0}";
        }
    }
}