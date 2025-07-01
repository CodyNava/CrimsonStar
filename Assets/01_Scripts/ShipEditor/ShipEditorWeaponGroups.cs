using System.Collections.Generic;
using UnityEngine;


public class ShipEditorWeaponGroups : MonoBehaviour
{
    [Header("WeaponGroups")] [SerializeField]
    private int currentGroup;

    [SerializeField] public float buttonSelectedSizeIncrease;
    [SerializeField] public Color selectedColor;
    [SerializeField] public Color deSelectedColor;
    [SerializeField] private Camera mainCamera;
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

    public void ChangeMaskForEachGroup(int groupID)
    {
        var weaponGroupOneLayer = LayerMask.NameToLayer("WeaponGroupOne");
        var weaponGroupTwoLayer = LayerMask.NameToLayer("WeaponGroupTwo");
        var weaponGroupThreeLayer = LayerMask.NameToLayer("WeaponGroupThree");
        
        switch (currentGroup)
        {
            case 1:
                mainCamera.cullingMask += weaponGroupOneLayer;
                mainCamera.cullingMask -= weaponGroupTwoLayer;
                mainCamera.cullingMask -= weaponGroupThreeLayer;
                break;
            case 2:
                mainCamera.cullingMask -= weaponGroupOneLayer;
                mainCamera.cullingMask += weaponGroupTwoLayer;
                mainCamera.cullingMask -= weaponGroupThreeLayer;
                break;
            case 3:
                mainCamera.cullingMask -= weaponGroupOneLayer;
                mainCamera.cullingMask -= weaponGroupTwoLayer;
                mainCamera.cullingMask += weaponGroupThreeLayer;
                break;
        }
    }
    
    public void AddModuleToWeaponGroup(NetEditorModule module)
    {
        if (module.ModuleData.ModuleCategory != NetModuleCategory.Weapons) return;
        switch (currentGroup)
        {
            case 1:
                weaponGroupOne.Add(module);
                module.gameObject.layer = LayerMask.NameToLayer("WeaponGroupOne");
                break;
            case 2:
                weaponGroupTwo.Add(module);
                module.gameObject.layer = LayerMask.NameToLayer("WeaponGroupTwo");
                break;
            case 3:
                weaponGroupThree.Add(module);
                module.gameObject.layer = LayerMask.NameToLayer("WeaponGroupThree");
                break;
        }
    }
    
    public void RemoveModuleFromWeaponGroup(NetEditorModule module)
    {
        if (module.ModuleData.ModuleCategory != NetModuleCategory.Weapons) return;
        weaponGroupOne.Remove(module);
        weaponGroupTwo.Remove(module);
        weaponGroupThree.Remove(module);
        module.gameObject.layer = LayerMask.NameToLayer("Outline");

    }
}