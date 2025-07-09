using System;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using TMPro;

public class ShipEditorTimerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timerDisplay;
    [SerializeField] private Image timerButtonSprite;
    [SerializeField] private float noTimeColorThreshold;

    public void Update()
    {
        DisplayTimer();
    }

    void DisplayTimer()
    {
        if (InstanceFinder.HasInstance<NetShipEditorConductor>())
        {
            float remainingTime = NetShipEditorConductor.Instance.TimeRemaining;
            if (remainingTime < noTimeColorThreshold) timerButtonSprite.color = Color.white;
            timerDisplay.text = $"{remainingTime:0}";
        }
    }
}