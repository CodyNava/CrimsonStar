using System;
using System.Text;
using TMPro;
using UnityEngine;

public class ShipEditorCurrencyDisplay : MonoBehaviour
{
    [SerializeField] private ShipEditor shipEditor;
    [SerializeField] private TMP_Text currency;
    private StringBuilder stringBuilder = new();

    void Update()
    {
        NetShipEditorData netShipEditorData = shipEditor.NetShipEditorData;
        if (netShipEditorData == null)
        {
            return;
        }
        stringBuilder.Clear();
        foreach (int currencyNumber in Enum.GetValues(typeof(NetCurrencyType)))
        {
            NetCurrencyType currencyType = (NetCurrencyType)currencyNumber;
            int count = netShipEditorData.ResourceStorage.C_GetRemainingResourceCount(currencyType);
            stringBuilder.AppendLine($"{count} {currencyType}");
        }
        currency.text = stringBuilder.ToString();
    }
}