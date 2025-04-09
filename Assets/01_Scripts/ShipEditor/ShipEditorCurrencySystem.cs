using TMPro;
using UnityEngine;

public class ShipEditorCurrencySystem : MonoBehaviour
{
    [SerializeField] int _currency = 1000;
    public TextMeshProUGUI _currencytext;

    void Start()
    {
        _currencytext.text = "Currency: " + _currency.ToString();
    }

    public void PayCurrency(int cost)
    {
        _currency -= cost;
        _currencytext.text = "Currency: " + _currency.ToString();
    }

    public void AddCurrency(int cost)
    {
        _currency += cost;
        _currencytext.text = "Currency: " + _currency.ToString();
    }

    public int GetCurrency()
    {
        return _currency;
    }
}