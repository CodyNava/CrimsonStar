using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NetEditorModule : MonoBehaviour
{
    private static readonly int ColourShift = Shader.PropertyToID("_ColourShift");
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    [HideInInspector] public bool IsSelected { get; set; }
    public HexCoordinate PlacedLocation { get; set; }
    public int PlacedRotation { get; set; }
    public NetModuleData ModuleData => ModuleID.GetModuleData();
    public List<HexCoordinate> LocalCoordinates { get; private set; }
    [HideInInspector] public bool IsPowered { get; set; }
    [field: SerializeField] private GameObject PowerMaterialGameObject { get; set; }
    private Material PowerMaterial { get; set; }
    private ShipEditor ShipEditor { get; set; }
    private ShipEditorWeaponGroups WeaponGroupManager { get; set; }
    private int CurrentGroup => WeaponGroupManager.currentGroup;
    [Tooltip("poweredColor is only Relevant if it can be Powered")]
    [field: SerializeField] private Color32 poweredColor;
    private Color32 _originalColor;


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
        ShipEditor = FindFirstObjectByType<ShipEditor>();
        WeaponGroupManager = FindFirstObjectByType<ShipEditorWeaponGroups>();
        if (!ModuleData.CanBePowered) return;
        var mesh = GetComponentInChildren<MeshRenderer>();
        PowerMaterial = mesh.materials[1];
        _originalColor = PowerMaterial.GetColor(ColourShift);
    }

    public void PickUpModule()
    {
        ShipEditor.RemoveModule(this);
    }

    public void ModuleSelected()
    {
        ShipEditor.moduleFirstSelectedGP = true;
        VisualTransform.gameObject.layer =
            IsSelected ? LayerMask.NameToLayer("Outline") : LayerMask.NameToLayer("Modules");
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
                PowerMaterial.SetColor(ColourShift, IsPowered ? poweredColor : _originalColor);
            }
        }
        else if (PowerMaterial.GetColor(ColourShift) != _originalColor)
        {
            PowerMaterial.SetColor(ColourShift, _originalColor) ;
        }

        IsPowered = Powered();
    }

    private void ChangeLayerBasedOnWeaponGroup()
    {
        if (!ShipEditor.EditorModuleList.Contains(this)) return;

        var weaponGroupOne = WeaponGroupManager.weaponGroupOne;
        var weaponGroupTwo = WeaponGroupManager.weaponGroupTwo;
        var weaponGroupThree = WeaponGroupManager.weaponGroupThree;

        var inGroupOneAndGroupActive = weaponGroupOne.Contains(this) && CurrentGroup == 1 && !IsSelected;
        var inGroupTwoAndGroupActive = weaponGroupTwo.Contains(this) && CurrentGroup == 2 && !IsSelected;
        var inGroupThreeAndGroupActive = weaponGroupThree.Contains(this) && CurrentGroup == 3 && !IsSelected;
        
        var weaponLayer = LayerMask.NameToLayer("WeaponGroupOne");
        var normalLayer = IsSelected ? LayerMask.NameToLayer("Outline") : LayerMask.NameToLayer("Modules");

        VisualTransform.gameObject.layer = inGroupOneAndGroupActive ? weaponLayer : normalLayer;
        if (inGroupOneAndGroupActive) return;
        VisualTransform.gameObject.layer = inGroupTwoAndGroupActive ? weaponLayer : normalLayer;
        if (inGroupTwoAndGroupActive) return;
        VisualTransform.gameObject.layer = inGroupThreeAndGroupActive ? weaponLayer : normalLayer;
    }

    public bool EnergyViewEnable() => ShipEditor.inEnergyView;
    public bool Powered() => ShipEditor.CheckIfPowered(PlacedLocation);
}