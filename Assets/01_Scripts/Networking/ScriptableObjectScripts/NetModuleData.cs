using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Ship.Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "NetModuleData", menuName = "Modules/Net Module Data")]
public class NetModuleData : ScriptableObject
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }
    [field: SerializeField] public NetModuleCategory ModuleCategory { get; private set; }

    [field: SerializeField]
    public List<Vector3Int> LocalModuleCoordinates { get; private set; } = new()
    {
        new Vector3Int(0, 0, 0)
    };

    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public NetModuleBaseStats BaseStats { get; private set; }
    [field: SerializeField] public bool CanRotate { get; private set; }

    [field: SerializeField] public NetEditorModule ShipEditorPrefab { get; private set; }
    [field: SerializeField] public NetGameplayModule GameplayPrefab { get; private set; }
    [field: SerializeField] public GameObject VisualModelPrefab { get; private set; }
    
    [Header("UI")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public float HexagonSize { get; private set; }
    
    
    public IEnumerable<HexCoordinate> GetLocalHexCoordinates() =>
        LocalModuleCoordinates.Select(vec => new HexCoordinate(vec));

    // TODO: Provide ability to define connectionEdges/valid connection neighbours or inverse
    public IEnumerable<HexCoordinate> GetLocalNeighbourCoordinates()
    {
        HashSet<HexCoordinate> neighbourCoordinates = new HashSet<HexCoordinate>();
        foreach (HexCoordinate localHexCoordinate in GetLocalHexCoordinates())
        {
            neighbourCoordinates.UnionWith(localHexCoordinate.Neighbors());
        }
        neighbourCoordinates.ExceptWith(GetLocalHexCoordinates());

        return neighbourCoordinates;
    }
}
