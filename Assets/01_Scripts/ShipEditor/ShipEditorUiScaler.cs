using System;
using UnityEngine;

public class ShipEditorUiScaler : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    public void Update()
    {
        AdjustCanvasScale();
    }
    public void AdjustCanvasScale()
    {
        float referenceWidth = 3840f;
        float referenceHeight = 2160f;
        float screenRatioX = Screen.width;
        float screenRatioY = Screen.height;

        bool isGreaterThanTwoTimes = screenRatioX > screenRatioY * 2f;
        bool isGreaterThanThreeTimes = screenRatioX > screenRatioY * 3f;
        
        bool isSixteenTen = Mathf.Abs((screenRatioX / screenRatioY) - (16f / 10f)) < 0.05f;

        RectTransform rt = canvas.GetComponent<RectTransform>();

        if (isGreaterThanThreeTimes)
        {
            rt.sizeDelta = new Vector2(referenceWidth * 2f, referenceHeight);
        }
        else if (isGreaterThanTwoTimes)
        {
            rt.sizeDelta = new Vector2(5120f, referenceHeight);
        }
        else if (isSixteenTen)
        {
            rt.sizeDelta = new Vector2(referenceWidth * (16f / 10f) / (16f / 9f), referenceHeight);
        }
        else
        {
            rt.sizeDelta = new Vector2(referenceWidth, referenceHeight);
        }
    }
}