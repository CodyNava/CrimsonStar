using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class TooltipBehaviour : MonoBehaviour
{
    public static TooltipBehaviour instance;
    public TextMeshProUGUI tooltipText;
    private Tooltip _tooltipObject;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowToolTip(string tooltip)
    {
        gameObject.SetActive(true);
        tooltipText.text = tooltip;
        Debug.Log("Tooltip shown");
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        tooltipText.text = string.Empty;
    }
}