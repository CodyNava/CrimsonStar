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
        bool isGreaterThanTwoTimes = screenRatioX > screenRatioY * 2;
        bool isGreaterThanThreeTimes = screenRatioX > screenRatioY * 3;
        RectTransform rt = canvas.GetComponent<RectTransform>();
        if (!isGreaterThanThreeTimes)
            rt.sizeDelta = isGreaterThanTwoTimes
            ? new Vector2(5120, referenceHeight)
            : new Vector2(referenceWidth, referenceHeight);
        else
        {
            rt.sizeDelta = new Vector2(referenceWidth * 2, referenceHeight);
        }
    }
}