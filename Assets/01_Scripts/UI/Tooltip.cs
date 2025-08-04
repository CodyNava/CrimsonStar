using System;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [Header("Tooltip")]
    [Tooltip("Enter your Tooltip here")]
    public string statOne;
    public string statTwo;
    public string statThree;
    public string statFour;
    public string statFive;
    public string statSix;
    public Sprite statOneImage;
    public Sprite statTwoImage;
    public Sprite statThreeImage;
    public Sprite statFourImage;
    public Sprite statFiveImage;
    public Sprite statSixImage;

    [Header("DO not Touch")]
    [SerializeField] private RectTransform buttonRectTransform;
    private Camera _camera;
    
    public void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        _camera = FindFirstObjectByType<Camera>();
        Vector3[] corners = new Vector3[4];
        buttonRectTransform.GetWorldCorners(corners);
        Vector3 topRightWorld = corners[2];

        Vector3 screenPos = _camera.WorldToScreenPoint(topRightWorld);
        if (statOne == String.Empty) return;
        TooltipBehaviour.Instance.ShowToolTipOne(statOne, statOneImage);
        TooltipBehaviour.Instance.transform.position = screenPos;
        if (statTwo == String.Empty) return;
        TooltipBehaviour.Instance.ShowToolTipTwo(statTwo, statTwoImage);
        TooltipBehaviour.Instance.transform.position = screenPos;
        if (statThree == String.Empty) return;
        TooltipBehaviour.Instance.ShowToolTipThree(statThree, statThreeImage);
        TooltipBehaviour.Instance.transform.position = screenPos;
        if (statFour == String.Empty) return;
        TooltipBehaviour.Instance.ShowToolTipFour(statFour, statFourImage);
        TooltipBehaviour.Instance.transform.position = screenPos;
        if (statFive == String.Empty) return;
        TooltipBehaviour.Instance.ShowToolTipFive(statFive, statFiveImage);
        TooltipBehaviour.Instance.transform.position = screenPos;
        if (statSix == String.Empty) return;
        TooltipBehaviour.Instance.ShowToolTipSix(statSix, statSixImage);
        TooltipBehaviour.Instance.transform.position = screenPos;
    }

    public void OnMouseExit()
    {
        Debug.Log("OnMouseEnter");
        TooltipBehaviour.Instance.HideToolTip();
    }
}
