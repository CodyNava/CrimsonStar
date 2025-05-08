using System.Text;
using TMPro;
using UnityEngine;

public class ShipEditorButton : MonoBehaviour
{
    [SerializeField] private NetModuleID netModuleID;
    [SerializeField] private ShipEditor shipEditor;
    [SerializeField] private TMP_Text costText;

    public void ButtonClick()
    {
        shipEditor.SpawnPart(netModuleID);
    }

    private void OnEnable()
    {
        StringBuilder sb = new StringBuilder();
        NetModuleData netModuleData = netModuleID.GetModuleData();
        foreach ((NetCurrencyType currency, int cost) in netModuleData.Costs)
        {
            sb.Append($"{cost} {DataProvider.Instance.CurrencyConfig.CurrencyDisplayInfos[currency].abbreviation}, ");
        }
        if (sb.Length > 0)
        {
            sb.Remove(sb.Length - 2, 2);
        }
        costText.text = sb.ToString();
    }
}