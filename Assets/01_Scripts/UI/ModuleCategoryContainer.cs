using System;
using System.Collections.Generic;
using UnityEngine;

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
            var colorPresets = new List<List<Color>>
            {
                presetData.presetColor1,
                presetData.presetColor2,
                presetData.presetColor3,
                presetData.presetColor4,
                presetData.presetColor5,
                presetData.presetColor6
            };


            for (int i = 0; i < colorPresets.Count; i++)
            {
                switch (i)
                {
                    case 0: colorPresetButton.SetColor(presetData.presetColor1); break;
                    case 1: colorPresetButton.SetColor(presetData.presetColor2); break;
                    case 2: colorPresetButton.SetColor(presetData.presetColor3); break;
                    case 3: colorPresetButton.SetColor(presetData.presetColor4); break;
                    case 4: colorPresetButton.SetColor(presetData.presetColor5); break;
                    case 5: colorPresetButton.SetColor(presetData.presetColor6); break;
                }
                colorPresetButton.SetName( presetData, i);
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