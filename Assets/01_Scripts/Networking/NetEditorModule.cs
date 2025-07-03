using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetEditorModule : MonoBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    public HexCoordinate PlacedLocation { get; set; }
    public int PlacedRotation { get; set; }
    public NetModuleData ModuleData => ModuleID.GetModuleData();
    public List<HexCoordinate> LocalCoordinates { get; private set; }
    [field: SerializeField] public bool IsPowered { get; set; }
    [field: SerializeField] public GameObject PowerMaterialGameObject { get; set; }
    [field: SerializeField] public Material PowerMaterial { get; set; }
    [field: SerializeField] public ShipEditor shipEditor { get; set; }
    [field: SerializeField] public ShipEditorWeaponGroups WeaponGroupManager { get; set; }
    [field: SerializeField] public GameObject WeaponGroupOutliner { get; set; }
    [field: SerializeField] public int currentGroup => WeaponGroupManager.currentGroup;
    [field: SerializeField] public bool isSelected { get; set; }
    private Color originalColor;

    public void Initialize()
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[ModuleID];
        LocalCoordinates = new List<HexCoordinate>();
        foreach (Vector3Int localCoordinate in moduleData.LocalModuleCoordinates)
        {
            LocalCoordinates.Add(new HexCoordinate(localCoordinate.x, localCoordinate.y, localCoordinate.z));
        }
    }

    public void Awake()
    {
        shipEditor = FindFirstObjectByType<ShipEditor>();
        WeaponGroupManager = FindFirstObjectByType<ShipEditorWeaponGroups>();
        PowerMaterial = GetComponentInChildren<MeshRenderer>().material;
        if (ModuleData.CanBePowered) originalColor = PowerMaterial.color;
    }

    public void PickUpModule()
    {
        shipEditor.RemoveModule(this);
    }

    public void ModuleSelected()
    {
        shipEditor.moduleFirstSelectedGP = true;
        VisualTransform.gameObject.layer =
            isSelected ? LayerMask.NameToLayer("Outline") : LayerMask.NameToLayer("Modules");
    }

    public void C_RotateClockwise()
    {
        for (int i = 0; i < LocalCoordinates.Count; i++)
        {
            LocalCoordinates[i] = LocalCoordinates[i].RotateClockwise();
        }

        PlacedRotation++;
        if (PlacedRotation > 5)
        {
            PlacedRotation -= 6;
        }

        C_UpdateRotation();
    }

    public void C_RotateCounterclockwise()
    {
        for (int i = 0; i < LocalCoordinates.Count; i++)
        {
            LocalCoordinates[i] = LocalCoordinates[i].RotateCounterClockwise();
        }

        PlacedRotation--;
        if (PlacedRotation < 0)
        {
            PlacedRotation += 6;
        }

        C_UpdateRotation();
    }

    private void C_UpdateRotation()
    {
        transform.rotation = Quaternion.AngleAxis(PlacedRotation * 60, Vector3.back);
    }

    public void Update()
    {
        if (ModuleData.CanBePowered) ChangeMaterialAndCheckPowerAlways();
        if (ModuleData.ModuleCategory == NetModuleCategory.Weapons) ChangeLayerBasedOnWeaponGroup();
    }

    private void ChangeMaterialAndCheckPowerAlways()
    {
        if (EnergyViewEnable())
        {
            if (ModuleData.CanBePowered)
            {
                PowerMaterial.color = IsPowered ? Color.green : Color.blue;
            }
        }
        else if (PowerMaterial.color != originalColor)
        {
            PowerMaterial.color = originalColor;
        }

        IsPowered = Powered();
    }

    private void ChangeLayerBasedOnWeaponGroup()
    {
        if (!shipEditor.EditorModuleList.Contains(this)) return;

        var weaponGroupOne = WeaponGroupManager.weaponGroupOne;
        var weaponGroupTwo = WeaponGroupManager.weaponGroupTwo;
        var weaponGroupThree = WeaponGroupManager.weaponGroupThree;

        var inGroupOneAndGroupActive = weaponGroupOne.Contains(this) && currentGroup == 1 && !isSelected;
        var inGroupTwoAndGroupActive = weaponGroupTwo.Contains(this) && currentGroup == 2 && !isSelected;
        var inGroupThreeAndGroupActive = weaponGroupThree.Contains(this) && currentGroup == 3 && !isSelected;
        var weaponLayer = LayerMask.NameToLayer("WeaponGroupOne");
        var normalLayer = isSelected ? LayerMask.NameToLayer("Outline") : LayerMask.NameToLayer("Modules");

        VisualTransform.gameObject.layer = inGroupOneAndGroupActive ? weaponLayer : normalLayer;
        if (inGroupOneAndGroupActive) return;
        VisualTransform.gameObject.layer = inGroupTwoAndGroupActive ? weaponLayer : normalLayer;
        if (inGroupTwoAndGroupActive) return;
        VisualTransform.gameObject.layer = inGroupThreeAndGroupActive ? weaponLayer : normalLayer;
    }

    public bool EnergyViewEnable() => shipEditor.inEnergyView;

    //public bool ReactorNearby() => shipEditor.IsPowereableInRangeOfReactor();
    public bool Powered() => shipEditor.CheckIfPowered(PlacedLocation);
}