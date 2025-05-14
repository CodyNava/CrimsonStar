using System;
using UnityEngine;

public class ModuleCategoryContainer : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private ModuleSelectionButton selectionButton;
    public void SetModuleCategory(NetModuleCategory category)
    {
        ClearButtons();

        foreach (int idInt in Enum.GetValues(typeof(NetModuleID)))
        {
            NetModuleID id = (NetModuleID)idInt;
            if (id == NetModuleID.Unknown)
            {
                continue;
            }
            NetModuleData moduleData = id.GetModuleData();
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
        for (int i = 0; i < containerChildren; i++)
        {
            Destroy(container.transform.GetChild(0).gameObject);
        }
    }
}
