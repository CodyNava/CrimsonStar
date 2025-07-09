using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;


public class ShipEditorWeaponGroups : MonoBehaviour
{
    public float buttonSelectedSizeIncrease;
    [SerializeField] private Camera mainCamera;

    [Header("WeaponGroups")] public int currentGroup;

    [HideInInspector] public List<ShipEditorWeaponGroupButton> weaponGroupButtons =
        new List<ShipEditorWeaponGroupButton>();

    [SerializeField] public List<NetEditorModule> weaponGroupOne =
        new List<NetEditorModule>();

    [SerializeField] public List<NetEditorModule> weaponGroupTwo =
        new List<NetEditorModule>();

    [SerializeField] public List<NetEditorModule> weaponGroupThree =
        new List<NetEditorModule>();
    [SerializedDictionary]


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

    public void AddModuleToWeaponGroup(NetEditorModule module, HexCoordinate coord)
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
        NetModuleWeaponGroupData.WriteWeaponGroup(coord, currentGroup);
        module.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Modules");
    }

    public void RemoveModuleFromWeaponGroup(NetEditorModule module, HexCoordinate coord)
    {
        if (module.ModuleData.ModuleCategory != NetModuleCategory.Weapons) return;
        weaponGroupOne.Remove(module);
        weaponGroupTwo.Remove(module);
        weaponGroupThree.Remove(module);
        module.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Outline");
        NetModuleWeaponGroupData.RemoveWeaponGroup(coord);
    }
}