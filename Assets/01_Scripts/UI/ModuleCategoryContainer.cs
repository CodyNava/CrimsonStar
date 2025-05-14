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
            NetModuleData moduleData = id.GetModuleData();
            if (moduleData.ModuleCategory != category)
            {
                continue;
            }
            ModuleSelectionButton selectedButton = Instantiate(selectionButton, container.transform);
            selectionButton.Configure(moduleData);
            //Todo add vertical layout group to container
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
