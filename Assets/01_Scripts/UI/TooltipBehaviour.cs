using UnityEngine;
using TMPro;

public class TooltipBehaviour : MonoBehaviour
{
    public static TooltipBehaviour Instance;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI advancedTooltipText;
    [SerializeField] private TextMeshProUGUI healthTooltipText;
    //[SerializeField] private Sprite moduleIconToolTip;

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
    public void ShowAdvancedToolTip (string tooltip)
    {
        advancedTooltipText.text = tooltip;
        gameObject.SetActive(true);
    }
    public void ShowHealthTip (string tooltip)
    {
        healthTooltipText.text = tooltip;
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        tooltipText.text = string.Empty;
        advancedTooltipText.text = string.Empty;
        healthTooltipText.text = string.Empty;
        //moduleIconToolTip = null;
    }
}