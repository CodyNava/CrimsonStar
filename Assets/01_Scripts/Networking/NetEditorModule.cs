using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        PowerMaterial = GetComponentInChildren<MeshRenderer>().material;
        originalColor = PowerMaterial.color;
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
        ChangeMaterialAndCheckPower();
        if (!EnergyViewEnable() && PowerMaterial.color != originalColor)
        {
            PowerMaterial.color = originalColor;
        }
    }

    public void ChangeMaterialAndCheckPower()
    {
        if (ModuleData.CanBePowered)
        {
            IsPowered = shipEditor.CheckIfPowered(PlacedLocation);
            if (shipEditor.inEnergyView)
            {
                PowerMaterial.color = IsPowered ? Color.green : Color.blue;
            }
            //todo implement shader change (waiting for gd to decide)
        }
    }
    public bool EnergyViewEnable() => shipEditor.inEnergyView;
}