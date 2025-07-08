using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NetEditorModule : MonoBehaviour
{
    private static readonly int ColourShift = Shader.PropertyToID("_EmissionColor");
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    [HideInInspector] public bool IsSelected { get; set; }
    public HexCoordinate PlacedLocation { get; set; }
    public int PlacedRotation { get; set; }
    public NetModuleData ModuleData => ModuleID.GetModuleData();
    public List<HexCoordinate> LocalCoordinates { get; private set; }
    [HideInInspector] public bool IsPowered { get; set; }
    [field: SerializeField] private GameObject PowerMaterialGameObject { get; set; }
    [field: SerializeField] private Material PowerMaterial { get; set; }
    private ShipEditor ShipEditor { get; set; }
    private ShipEditorWeaponGroups WeaponGroupManager { get; set; }
    private int CurrentGroup => WeaponGroupManager.currentGroup;
    [SerializeField] private int _insideCurrentGroup;
    [SerializeField] private GameObject healthOverLayObject;
    [SerializeField] private Color _healthOverLayObjectColour;

    [Tooltip("poweredColor is only Relevant if it can be Powered")] [field: SerializeField]
    private Color poweredColor;

    [Tooltip("notPoweredColor is only Relevant if it can be Powered")] [field: SerializeField]
    private Color notPoweredColor;

    private Color _originalColor;
    private float _originalColorIntensity;


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
        if (ModuleData.ModuleID == NetModuleID.Bridge)
        {
            ShipEditorHealthOverlay.WriteHealthMap(PlacedLocation, ModuleData.BaseStats.health);
        }
        if (!ModuleData.CanBePowered) return;
        var powerMesh = GetComponentInChildren<MeshRenderer>();
        PowerMaterial = powerMesh.materials[3];
        _originalColor = PowerMaterial.GetColor(ColourShift);
        _originalColorIntensity = Mathf.Max(_originalColor.r, _originalColor.g, _originalColor.b);
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

    public void TotalHealthChangeOverLayColour()
    {
        var newColor = Color.white.WithAlpha(0.2f);
        
        bool lowTotalHealth = ModuleData.BaseStats.health >= 0f;
        bool midTotalHealth = ModuleData.BaseStats.health >= 90f;
        bool highTotalHealth = ModuleData.BaseStats.health >= 150f;

        if (lowTotalHealth) newColor = Color.red.WithAlpha(0.2f);
        if (midTotalHealth) newColor = Color.yellow.WithAlpha(0.2f);
        if (highTotalHealth) newColor = Color.green.WithAlpha(0.2f);

        _healthOverLayObjectColour = newColor;
        healthOverLayObject.GetComponent<MeshRenderer>().material.color = _healthOverLayObjectColour;
    }

    public void PercentageHealthChangeOverLayColour(float colorValue)
    {
        
        if (!healthOverLayObject) return;
        var newColor = Color.Lerp(Color.red.WithAlpha(0.2f), Color.green.WithAlpha(0.2f), colorValue);
         _healthOverLayObjectColour = newColor;
        healthOverLayObject.GetComponent<MeshRenderer>().material.color = _healthOverLayObjectColour;
         Debug.Log("ColorValue ===  "+colorValue);
    }

    private void ChangeMaterialAndCheckPowerAlways()
    {
        if (EnergyViewEnable())
        {
            if (ModuleData.CanBePowered)
            {
                PowerMaterial.SetColor(ColourShift, IsPowered
                    ? poweredColor * _originalColorIntensity
                    : notPoweredColor * _originalColorIntensity);
            }
        }
        else if (PowerMaterial.GetColor(ColourShift) != _originalColor)
        {
            PowerMaterial.SetColor(ColourShift, _originalColor);
        }

        IsPowered = Powered();
    }

    private void ChangeLayerBasedOnWeaponGroup()
    {
        _insideCurrentGroup = NetModuleWeaponGroupData.ReadWeaponGroup(PlacedLocation);

        AddToListWhenReconstructing();

        if (!ShipEditor.EditorModuleList.Contains(this)) return;
        var inGroupOneAndGroupActive = _insideCurrentGroup == 1 && CurrentGroup == 1 && !IsSelected;
        var inGroupTwoAndGroupActive = _insideCurrentGroup == 2 && CurrentGroup == 2 && !IsSelected;
        var inGroupThreeAndGroupActive = _insideCurrentGroup == 3 && CurrentGroup == 3 && !IsSelected;

        var weaponLayer = LayerMask.NameToLayer("WeaponGroupOne");
        var normalLayer = IsSelected ? LayerMask.NameToLayer("Outline") : LayerMask.NameToLayer("Modules");

        VisualTransform.gameObject.layer = inGroupOneAndGroupActive ? weaponLayer : normalLayer;
        if (inGroupOneAndGroupActive) return;
        VisualTransform.gameObject.layer = inGroupTwoAndGroupActive ? weaponLayer : normalLayer;
        if (inGroupTwoAndGroupActive) return;
        VisualTransform.gameObject.layer = inGroupThreeAndGroupActive ? weaponLayer : normalLayer;
    }

    public void AddToListWhenReconstructing()
    {
        if (!ShipEditor.joiningEditor) return;
        bool notInAnyList = !WeaponGroupManager.weaponGroupOne.Contains(this) &&
                            !WeaponGroupManager.weaponGroupTwo.Contains(this) &&
                            !WeaponGroupManager.weaponGroupThree.Contains(this);
        if (notInAnyList)
        {
            switch (_insideCurrentGroup)
            {
                case 1:
                    WeaponGroupManager.weaponGroupOne.Add(this);
                    break;
                case 2:
                    WeaponGroupManager.weaponGroupTwo.Add(this);
                    break;
                case 3:
                    WeaponGroupManager.weaponGroupThree.Add(this);
                    break;
            }
        }
        else if (_insideCurrentGroup == 0)
        {
            WeaponGroupManager.weaponGroupOne.Remove(this);
            WeaponGroupManager.weaponGroupTwo.Remove(this);
            WeaponGroupManager.weaponGroupThree.Remove(this);
        }
    }

    public bool EnergyViewEnable() => ShipEditor.inEnergyView;
    public bool Powered() => ShipEditor.CheckIfPowered(PlacedLocation);
}