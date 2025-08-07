using System;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using TMPro;

public class ShipEditorTimerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timerDisplay;
    [SerializeField] private TMP_Text timerHeader;
    [SerializeField] private Image timerButtonImage;
    [SerializeField] private Image hourGlassImage;
    [SerializeField] private float noTimeColorThreshold;
    [SerializeField] private string timerHeaderStringNotReady, timerHeaderStringReady;
    [SerializeField] private ShipEditor shipEditor;
    private Button _thisButton;
    [SerializeField] private Color startColor, endColor, blinkColor;
    private float _maxTime;

    public void Update() => DisplayTimer();
    public void Start() => Resetting();
    public void Awake() => timerHeader.text = timerHeaderStringNotReady;

    private void Resetting()
    {
        timerButtonImage.color = Color.gray;
        _maxTime = 0f;
        shipEditor.blockingPlane.gameObject.SetActive(false);
    }

    void DisplayTimer()
    {
        if (InstanceFinder.HasInstance<NetShipEditorConductor>())
        {
            float remainingTime = NetShipEditorConductor.Instance.TimeRemaining;
            timerDisplay.text = $"{remainingTime:0}";
            //if (remainingTime == 0f) return;
            ChangeHourGlassAndTextColor(remainingTime);
            ChangeButtonColorConstantly(remainingTime);
            ChangeTimerHeaderText(remainingTime);
            ButtonBlinkIfThreshold(remainingTime);
            LerpButtonColorBackIfNoTime(remainingTime);
            DisableInteractions(remainingTime);
        }
    }

    private void ChangeHourGlassAndTextColor(float timer)
    {
        if (_maxTime <= 0f) _maxTime = timer;
        var t = 1f - Mathf.Clamp01(timer / _maxTime);
        var colorLerp = Color.Lerp(startColor, endColor, t);
        timerDisplay.color = colorLerp;
        hourGlassImage.color = colorLerp;
    }

    private void ChangeButtonColorConstantly(float timer)
    {
        if (timer < noTimeColorThreshold || shipEditor.IsReady) return;
        var adjustedMaxTime = _maxTime - noTimeColorThreshold;
        var t = 1f - Mathf.Clamp01((timer - noTimeColorThreshold) / adjustedMaxTime);
        var colorLerp2 = Color.Lerp(Color.gray, Color.white, t);
        timerButtonImage.color = colorLerp2;
    }

    private void LerpButtonColorBackIfNoTime(float timer)
    {
        if (Mathf.Approximately(timer, 0.1f) || shipEditor.IsReady)
        {
            var colorLerp = Color.Lerp(timerButtonImage.color, Color.gray, Time.deltaTime);
            timerButtonImage.color = colorLerp;
        }
    }

    private void ButtonBlinkIfThreshold(float timer)
    {
        if (timer > noTimeColorThreshold) return;
        var timerBasedValue = 7f + timer;
        var sinT = Mathf.Sin(Time.time * (20f / timerBasedValue)) * 0.5f + 0.5f;
        timerButtonImage.color = Color.Lerp(Color.white, blinkColor, sinT);
    }

    private void DisableInteractions(float timer) //
    {
        bool disableCheck = timer <= 0f || shipEditor.IsReady;
        if (!_thisButton) _thisButton = GetComponent<Button>();
        _thisButton.interactable = !disableCheck;
        shipEditor.blockingPlane.gameObject.SetActive(disableCheck);
    }

    private void ChangeTimerHeaderText(float timer)
    {
        timerHeader.text = shipEditor.IsReady || timer < 0.1f
            ? timerHeaderStringReady
            : timerHeaderStringNotReady;
    }
}