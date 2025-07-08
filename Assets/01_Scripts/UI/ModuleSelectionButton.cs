using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ModuleSelectionButton : MonoBehaviour
{
    [SerializeField] private Image moduleIcon;
    [SerializeField] private TMP_Text moduleName, sizeLabel, currencyLabel;
    [SerializeField] Tooltip tooltip;
    public static event Action<NetModuleID> ModuleSelected;
    private NetModuleID moduleID;


    public void SpawnModule()
    {
        ModuleSelected?.Invoke(moduleID);
    }

    public void Configure(NetModuleData data)
    {
        currencyLabel.text = $"{data.Cost}";
        moduleName.text = $"{data.DisplayName}";
        sizeLabel.text = $"{data.HexagonSize}";
        moduleIcon.sprite = data.Icon;
        moduleID = data.ModuleID;
        tooltip.message = data.HexagonSize.ToString();
        tooltip.healthMessage = $"{data.BaseStats.health.ToString()}";

    tooltip.advancedMessage = "test";
    }
}
