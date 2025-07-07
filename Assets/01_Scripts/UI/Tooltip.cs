using System;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string message;

    public void OnMouseEnter()
    {
        if (message == String.Empty) return;
        TooltipBehaviour.instance.transform.position = transform.position;
        TooltipBehaviour.instance.ShowToolTip(message);
    }

    public void OnMouseExit()
    {
        TooltipBehaviour.instance.HideToolTip();
    }
}
