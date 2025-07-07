using UnityEngine;
using TMPro;

public class TooltipBehaviour : MonoBehaviour
{
    public static TooltipBehaviour Instance;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    

    public void ShowToolTip (string tooltip)
    {
        tooltipText.text = tooltip;
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        tooltipText.text = string.Empty;
    }
}