using System;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [Header("Tooltip")]
    [Tooltip("Enter your Tooltip here")]
    public string message;
    public string healthMessage;
    public string advancedMessage;

    [Header("DO not Touch")]
    [SerializeField] private RectTransform buttonRectTransform;
    private Camera _camera;
    
    public void OnMouseEnter()
    {
        _camera = FindFirstObjectByType<Camera>();
        Vector3[] corners = new Vector3[4];
        buttonRectTransform.GetWorldCorners(corners);
        Vector3 topRightWorld = corners[2];

        Vector3 screenPos = _camera.WorldToScreenPoint(topRightWorld);
        if (message == String.Empty) return;
        TooltipBehaviour.Instance.ShowToolTip(message);
        TooltipBehaviour.Instance.transform.position = screenPos;
        if (healthMessage == String.Empty) return;
        TooltipBehaviour.Instance.ShowHealthTip(healthMessage);
        TooltipBehaviour.Instance.transform.position = screenPos;
        if (advancedMessage == String.Empty) return;
        TooltipBehaviour.Instance.ShowAdvancedToolTip(advancedMessage);
        TooltipBehaviour.Instance.transform.position = screenPos;
    }

    public void OnMouseExit()
    {
        TooltipBehaviour.Instance.HideToolTip();
    }
}
