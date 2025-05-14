using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ModuleSelectionButton : MonoBehaviour
{
    [SerializeField] private Image moduleIcon;
    [SerializeField] private TMP_Text moduleName, sizeLabel, currencyLabel;
    public static event Action<NetModuleID> ModuleSelected;
    private NetModuleID moduleID;

    public void SpawnModule()
    {
        ModuleSelected?.Invoke(moduleID);
    }
    public void Configure(NetModuleData data)
    {
        currencyLabel.text = $"{data.Costs[NetCurrencyType.Gold]}";
        moduleName.text = $"{data.DisplayName}";
        sizeLabel.text = $"{data.HexagonSize}";
        moduleIcon.sprite = data.Icon;
        moduleID = data.ModuleID;
    }
}
