using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipBehaviour : MonoBehaviour
{
    public static TooltipBehaviour Instance;
    [SerializeField] private TextMeshProUGUI statOneText;
    [SerializeField] private TextMeshProUGUI statTwoText;
    [SerializeField] private TextMeshProUGUI statThreeText;
    [SerializeField] private TextMeshProUGUI statFourText;
    [SerializeField] private TextMeshProUGUI statFiveText;
    [SerializeField] private TextMeshProUGUI statSixText;
    [SerializeField] private GameObject statOneImage;
    [SerializeField] private GameObject statTwoImage;
    [SerializeField] private GameObject statThreeImage;
    [SerializeField] private GameObject statFourImage;
    [SerializeField] private GameObject statFiveImage;
    [SerializeField] private GameObject statSixImage;
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


    public void ShowToolTipOne(string tooltip, Sprite tooltipImage)
    {
        statOneText.text = tooltip;
        statOneImage.SetActive(true);
        statOneImage.GetComponent<Image>().sprite = tooltipImage;
        gameObject.SetActive(true);
    }

    public void ShowToolTipTwo(string tooltip, Sprite tooltipImage)
    {
        statTwoText.text = tooltip;
        statTwoImage.SetActive(true);
        statTwoImage.GetComponent<Image>().sprite = tooltipImage;
        gameObject.SetActive(true);
    }

    public void ShowToolTipThree(string tooltip, Sprite tooltipImage)
    {
        statThreeText.text = tooltip;
        statThreeImage.SetActive(true);
        statThreeImage.GetComponent<Image>().sprite = tooltipImage;
        gameObject.SetActive(true);
    }

    public void ShowToolTipFour(string tooltip, Sprite tooltipImage)
    {
        statFourText.text = tooltip;
        statFourImage.SetActive(true);
        statFourImage.GetComponent<Image>().sprite = tooltipImage;
        gameObject.SetActive(true);
    }

    public void ShowToolTipFive(string tooltip, Sprite tooltipImage)
    {
        statFiveText.text = tooltip;
        statFiveImage.SetActive(true);
        statFiveImage.GetComponent<Image>().sprite = tooltipImage;
        gameObject.SetActive(true);
    }

    public void ShowToolTipSix(string tooltip, Sprite tooltipImage)
    {
        statSixText.text = tooltip;
        statSixImage.SetActive(true);
        statSixImage.GetComponent<Image>().sprite = tooltipImage;
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        statOneText.text = string.Empty;
        statTwoText.text = string.Empty;
        statThreeText.text = string.Empty;
        statFourText.text = string.Empty;
        statFiveText.text = string.Empty;
        statSixText.text = string.Empty;
        statOneImage.SetActive(false);
        statTwoImage.SetActive(false);
        statThreeImage.SetActive(false);
        statFourImage.SetActive(false);
        statFiveImage.SetActive(false);
        statSixImage.SetActive(false);
        
        
    }
}