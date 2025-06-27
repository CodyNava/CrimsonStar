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
        int cost = DataProvider.GetModuleCost(netModuleID);
        costText.text = $"{cost} Currency";
    }
}