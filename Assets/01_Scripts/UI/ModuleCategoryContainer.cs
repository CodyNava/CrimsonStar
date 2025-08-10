using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModuleCategoryContainer : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private ModuleSelectionButton selectionButton;
    [SerializeField] private ColorPresetButton colorPresetButton;
    [SerializeField] private ColorPresetData presetData;
    [SerializeField] private GameObject colorPresetContainer;
    private string _currentName;


    public void SetModuleCategory(NetModuleCategory category)
    {
        ClearButtons();

        if (category == NetModuleCategory.ColorPresets)
        {
            foreach (var data in presetData.presets)
            {
                colorPresetButton.SetButtonSprite();
                colorPresetButton.SetColor(data.colors);
                colorPresetButton.SetName(data.presetName);
                Instantiate(colorPresetContainer, container.transform);
            }

            return;
        }

        foreach (int idInt in Enum.GetValues(typeof(NetModuleID)))
        {
            NetModuleID id = (NetModuleID)idInt;
            if (id == NetModuleID.Unknown)
            {
                continue;
            }

            NetModuleData moduleData = id.GetModuleData();
            if (!moduleData) continue;
            if (moduleData.ModuleCategory != category)
            {
                continue;
            }
            ModuleSelectionButton selectedButton = Instantiate(selectionButton, container.transform);
            selectedButton.Configure(moduleData);
            
        }

        if (!InputManager.Instance.IsGamepadUsed) return;
    }

    public void ClearButtons()
    {
        int containerChildren = container.transform.childCount;
        for (int i = containerChildren - 1; i >= 0; i--)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }
}