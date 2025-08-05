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
    [SerializeField] private Sprite timerButtonSpriteEscalation, timerButtonStartOriginSprite;
    [SerializeField] private float noTimeColorThreshold;
    [SerializeField] private string timerHeaderStringNotReady, timerHeaderStringReady;
    [SerializeField] private ShipEditor shipEditor;
    [SerializeField] private Color startColor, endColor;
    private float _maxTime;

    public void Update()
    {
        DisplayTimer();
    }

    public void Start()
    {
        Resetting();
    }

    private void Resetting()
    {
        timerButtonStartOriginSprite = timerButtonImage.sprite;
        _maxTime = 0f;
    }

    void DisplayTimer()
    {
        if (InstanceFinder.HasInstance<NetShipEditorConductor>())
        {
            float remainingTime = NetShipEditorConductor.Instance.TimeRemaining;
            timerDisplay.text = $"{remainingTime:0}";
            
            ChangeButtonColorTextSprite(remainingTime);
            ChangeHourGlassAndTextColor(remainingTime);
        }
    }

    private void ChangeButtonColorTextSprite(float timer)
    {
        if (timer < noTimeColorThreshold)
        {
            timerButtonImage.sprite = timerButtonSpriteEscalation;
            timerButtonImage.color = Color.white;
        }
        else
        {
            timerButtonImage.sprite = timerButtonStartOriginSprite;
            timerButtonImage.color = Color.gray;
        }

        timerHeader.text = shipEditor.IsReady
            ? timerHeaderStringReady
            : timerHeaderStringNotReady;
    }

    private void ChangeHourGlassAndTextColor(float timer)
    {
        if (_maxTime == 0f) _maxTime = timer;

        var t = 1f - Mathf.Clamp01(timer / _maxTime);
        var colorLerp = Color.Lerp(startColor, endColor, t);

        timerDisplay.color = colorLerp;
        hourGlassImage.color = colorLerp;
    }
}