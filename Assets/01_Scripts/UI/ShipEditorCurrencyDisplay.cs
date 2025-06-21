using System;
using System.Text;
using TMPro;
using UnityEngine;

public class ShipEditorCurrencyDisplay : MonoBehaviour
{
    [SerializeField] private ShipEditor shipEditor;
    [SerializeField] private TMP_Text currency;

    void Update()
    {
        NetMatchPlayer playerData = shipEditor.PlayerData;
        if (playerData == null)
        {
            return;
        }
        currency.text = $"Currency: {playerData.ResourceCount.Value}";
    }
}