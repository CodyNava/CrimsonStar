using System.Collections.Generic;
using UnityEngine;

public class NetEditorModule : MonoBehaviour
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    public HexCoordinate PlacedLocation { get; set; }
    public List<HexCoordinate> LocalCoordinates { get; private set; }

    private void Start()
    {
        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[ModuleID];
        LocalCoordinates = new List<HexCoordinate>();
        foreach (Vector3Int localCoordinate in moduleData.LocalModuleCoordinates)
        {
            LocalCoordinates.Add(new HexCoordinate(localCoordinate.x, localCoordinate.y, localCoordinate.z));
        }
    }

    public void RotateClockwise()
    {
        for (int i = 0; i < LocalCoordinates.Count; i++)
        {
            LocalCoordinates[i] = LocalCoordinates[i].RotateClockwise();
        }
    }

    public void RotateCounterclockwise()
    {
        for (int i = 0; i < LocalCoordinates.Count; i++)
        {
            LocalCoordinates[i] = LocalCoordinates[i].RotateCounterClockwise();
        }
    }
}
