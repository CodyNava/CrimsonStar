using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NetEditorModule : MonoBehaviour
{
    private static readonly int ColourShift = Shader.PropertyToID("_EmissionColor");
    private static readonly int Shift = Shader.PropertyToID("_ColourShift");
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public Transform VisualTransform { get; private set; }
    [HideInInspector] public bool IsSelected { get; set; }
    [field: SerializeField] public HexCoordinate PlacedLocation { get; set; }
    [field: SerializeField] public int PlacedRotation { get; set; }
    [field: SerializeField] public Transform CurrentTransform { get; set; }
    public NetModuleData ModuleData => ModuleID.GetModuleData();
    [field: SerializeField] public HealthOverLayData healthOverLayData;
    [field: SerializeField] public List<HexCoordinate> LocalCoordinates { get; private set; }
    [HideInInspector] public bool IsPowered { get; set; }
    private ShipEditor shipEditor { get; set; }
    private ShipEditorWeaponGroups WeaponGroupManager { get; set; }
    private int CurrentGroup => WeaponGroupManager.currentGroup;
    [SerializeField] private int _insideCurrentGroup;
    [SerializeField] private GameObject healthOverLayObject;
    [SerializeField] private Color _healthOverLayObjectColour;
    private Vector4 PresetColor1 { get; set; }
    private Vector4 PresetColor2 { get; set; }
    private Vector4 PresetColor3 { get; set; }
    private Material PresetMat1 { get; set; }
    private Material PresetMat2 { get; set; }
    private Material PresetMat3 { get; set; }
    private Material PresetMatHead1 { get; set; }
    private Material PresetMatHead2 { get; set; }
    private Material PresetMatHead3 { get; set; }
    [field: SerializeField] private GameObject PowerMaterialGameObject { get; set; }
    [field: SerializeField] private List<Material> MaterialsToColor { get; set; }
    [field: SerializeField] private List<Material> powerSocketMaterials;
    [field: SerializeField] private GameObject powerMaterialSocketsGameObject;
    [field: SerializeField] private GameObject PresetObject { get; set; }
    [field: SerializeField] private GameObject PresetObjectHead { get; set; }


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
        shipEditor = FindFirstObjectByType<ShipEditor>();
        WeaponGroupManager = FindFirstObjectByType<ShipEditorWeaponGroups>();
        if (ModuleData.ModuleID == NetModuleID.Bridge)
            ShipEditorHealthOverlay.WriteHealthMap(PlacedLocation, ModuleData.BaseStats.health);

        if (ModuleData.ModuleID != NetModuleID.Bridge) UpdateCurrentTransformRotation();
        GetPresetMaterials();
        GetPowerMaterial();
    }


    public void GetPowerMaterial()
    {
        if (!ModuleData.CanBePowered) return;

        _originalColor = Color.white;
        _originalColorIntensity = Mathf.Max(_originalColor.r, _originalColor.g, _originalColor.b);
        _originalColorIntensity *= 5f;

        switch (ModuleID)
        {
            case NetModuleID.TurretLaser:
                var socketMatLaser = powerMaterialSocketsGameObject.GetComponentInChildren<MeshRenderer>();
                powerSocketMaterials.Add(socketMatLaser.materials[1]);
                var headMatLaser = PresetObjectHead.GetComponent<MeshRenderer>().materials[3];
                powerSocketMaterials.Add(headMatLaser);
                break;
            
            case NetModuleID.DeepPenLaser:
                var socketMatDeep = powerMaterialSocketsGameObject.GetComponentInChildren<MeshRenderer>();
                powerSocketMaterials.Add(socketMatDeep.materials[1]);
                var headMatDeep = PresetObjectHead.GetComponent<MeshRenderer>().materials[3];
                powerSocketMaterials.Add(headMatDeep);
                break;
            
            case NetModuleID.ShredderGun:
                var socketMatShredder = powerMaterialSocketsGameObject.GetComponentInChildren<MeshRenderer>();
                powerSocketMaterials.Add(socketMatShredder.materials[1]);
                var baseMatShredder = PresetObject.GetComponent<MeshRenderer>().materials[3];
                powerSocketMaterials.Add(baseMatShredder);
                var headMatShredder = PresetObjectHead.GetComponent<MeshRenderer>().materials[2];
                powerSocketMaterials.Add(headMatShredder);
                break;
            
            case (NetModuleID.TurretRocketT2):
                var socketMatBattery = powerMaterialSocketsGameObject.GetComponentInChildren<MeshRenderer>();
                powerSocketMaterials.Add(socketMatBattery.materials[1]);
                var baseMatBattery = PresetObject.GetComponent<MeshRenderer>().materials[3];
                powerSocketMaterials.Add(baseMatBattery);
                var headMatBattery = PresetObjectHead.GetComponent<MeshRenderer>().materials[3];
                powerSocketMaterials.Add(headMatBattery);
                break;
        }
    }

    public void GetPresetMaterials()
    {
        PresetMat1 = PresetObject.GetComponent<MeshRenderer>().materials[0];
        PresetMat2 = PresetObject.GetComponent<MeshRenderer>().materials[1];
        PresetMat3 = PresetObject.GetComponent<MeshRenderer>().materials[2];

        if (!PresetObjectHead) return;

        PresetMatHead1 = PresetObjectHead.GetComponent<MeshRenderer>().materials[0];
        PresetMatHead2 = PresetObjectHead.GetComponent<MeshRenderer>().materials[1];
        PresetMatHead3 = PresetObjectHead.GetComponent<MeshRenderer>().materials[2];
    }

    public void SetMaterialsBasedOnPreset()
    {
        var presetName = shipEditor.PlayerData.SelectedPreset.Value;
        if (presetName == null) return;
        var currenPreset = DataProvider.GetColorPresetByName(presetName);
        PresetColor1 = currenPreset.color1;
        PresetColor2 = currenPreset.color2;
        PresetColor3 = currenPreset.color3;
    }

    private void UpdateMaterialPresets()
    {
        SetMaterialsBasedOnPreset();
        PresetMat1.SetVector(Shift, PresetColor1);
        PresetMat2.SetVector(Shift, PresetColor2);
        PresetMat3.SetVector(Shift, PresetColor3);
        if (PresetMatHead1 && PresetMatHead2 && PresetMatHead3)
        {
            PresetMatHead1.SetVector(Shift, PresetColor1);
            PresetMatHead2.SetVector(Shift, PresetColor2);
            PresetMatHead3.SetVector(Shift, PresetColor3);
        }
    }

    public void PickUpModule()
    {
        shipEditor.RemoveModule(this);
    }

    public void ModuleSelected()
    {
        shipEditor.moduleFirstSelectedGp = true;
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
        UpdateCurrentTransformRotation();
    }

    private void UpdateCurrentTransformRotation()
    {
        CurrentTransform = gameObject.transform;
    }


    public void Update()
    {
        if (ModuleData.CanBePowered) ChangeMaterialAndCheckPowerAlways();
        if (ModuleData.ModuleCategory == NetModuleCategory.Weapons) ChangeLayerBasedOnWeaponGroup();
        UpdateMaterialPresets();
    }

    public void TotalHealthChangeOverLayColour()
    {
        var newColor = Color.white;

        bool lowTotalHealth = healthOverLayData.LowHealth <= ModuleData.BaseStats.health;
        bool midTotalHealth = healthOverLayData.MidHealth <= ModuleData.BaseStats.health;
        bool highTotalHealth = healthOverLayData.HighHealth <= ModuleData.BaseStats.health;
        bool superHighTotalHealth = healthOverLayData.SuperHighHealth <= ModuleData.BaseStats.health;

        if (lowTotalHealth) newColor = healthOverLayData.LowHealthColor;
        if (midTotalHealth) newColor = healthOverLayData.MidHealthColor;
        if (highTotalHealth) newColor = healthOverLayData.HighHealthColor;
        if (superHighTotalHealth) newColor = healthOverLayData.SuperHighHealthColor;

        var healthOverlayMesh = healthOverLayObject.GetComponent<MeshRenderer>();

        _healthOverLayObjectColour = newColor;
        foreach (var material in healthOverlayMesh.materials)
        {
            material.color = _healthOverLayObjectColour;
        }
    }

    private void ChangeMaterialAndCheckPowerAlways()
    {
        if (EnergyViewEnable())
        {
            if (ModuleData.CanBePowered)
            {
                //PowerMaterial.SetColor(ColourShift, IsPowered
                   // ? poweredColor * _originalColorIntensity
                   // : notPoweredColor * _originalColorIntensity);

                foreach (var mat in powerSocketMaterials)
                {
                    mat.SetColor(ColourShift,
                        IsPowered
                            ? poweredColor * _originalColorIntensity
                            : notPoweredColor * _originalColorIntensity);
                }
            }
        }
        else
        {
            if (powerSocketMaterials.Count <= 0) return;
            foreach (var mat in powerSocketMaterials)
            {
                mat.SetColor(ColourShift, _originalColor);
            }
        }

        IsPowered = Powered();
    }

    private void ChangeLayerBasedOnWeaponGroup()
    {
        _insideCurrentGroup = NetModuleWeaponGroupData.ReadWeaponGroup(PlacedLocation);

        AddToListWhenReconstructing();

        if (!shipEditor.EditorModuleList.Contains(this)) return;
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
        if (!shipEditor.joiningEditor) return;
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

    public Transform GetRotation() => CurrentTransform;
    public bool EnergyViewEnable() => shipEditor.inEnergyView;

    private bool Powered()
    {
        var totalCoord = new List<HexCoordinate>();
        foreach (var coord in LocalCoordinates)
        {
            //if (coord == LocalCoordinates[0]) continue;
            totalCoord.Add(coord + PlacedLocation);
        }

        Debug.Log(totalCoord);

        return shipEditor.CheckIfPowered(totalCoord);
    }
}