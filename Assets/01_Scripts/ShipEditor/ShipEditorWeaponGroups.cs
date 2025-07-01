using System.Collections.Generic;
using UnityEngine;


public class ShipEditorWeaponGroups : MonoBehaviour
{
    [Header("WeaponGroups")] [SerializeField]
    private int currentGroup;

    [SerializeField] public float buttonSelectedSizeIncrease;
    [SerializeField] public Color selectedColor;
    [SerializeField] public Color deSelectedColor;
    [HideInInspector] public List<ShipEditorWeaponGroupButton> weaponGroupButtons =
        new List<ShipEditorWeaponGroupButton>();

    [SerializeField] public List<NetEditorModule> weaponGroupOne =
        new List<NetEditorModule>();

    [SerializeField] public List<NetEditorModule> weaponGroupTwo =
        new List<NetEditorModule>();

    [SerializeField] public List<NetEditorModule> weaponGroupThree =
        new List<NetEditorModule>();

    public void DeselectButtonsExcept(ShipEditorWeaponGroupButton activeButton)
    {
        foreach (var button in weaponGroupButtons)
        {
            if (button != activeButton)
            {
                button.ChangeToUnselected();
            }
        }
    }

    public void SetWeaponGroup(int id)
    {
        currentGroup = 0;
        currentGroup = id;
    }

    public void AddModuleToWeaponGroup(NetEditorModule module)
    {
        if (module.ModuleData.ModuleCategory != NetModuleCategory.Weapons) return;
        switch (currentGroup)
        {
            case 1:
                weaponGroupOne.Add(module);
                break;
            case 2:
                weaponGroupTwo.Add(module);
                break;
            case 3:
                weaponGroupThree.Add(module);
                break;
        }
    }
    
    public void RemoveModuleFromWeaponGroup(NetEditorModule module)
    {
        if (module.ModuleData.ModuleCategory != NetModuleCategory.Weapons) return;
        switch (currentGroup)
        {
            case 1:
                weaponGroupOne.Remove(module);
                break;
            case 2:
                weaponGroupTwo.Remove(module);
                break;
            case 3:
                weaponGroupThree.Remove(module);
                break;
        }
    }
}