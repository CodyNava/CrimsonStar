using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NetModuleData", menuName = "Modules/Net Module Data")]
public class NetModuleData : ScriptableObject
{
    [field: SerializeField] public NetModuleID ModuleID { get; private set; }

    [field: SerializeField]
    public List<Vector3Int> LocalModuleCoordinates { get; private set; } = new()
    {
        new Vector3Int(0, 0, 0)
    };

    [SerializedDictionary("Resource Type", "Cost")]
    [field: SerializeField] public SerializedDictionary<NetResourceType, int> Costs { get; private set; }
    [field: SerializeField] public NetModuleBaseStats BaseStats { get; private set; }
    [field: SerializeField] public bool CanRotate { get; private set; }

    [field: SerializeField] public NetEditorModule ShipEditorPrefab { get; private set; }
    [field: SerializeField] public NetGameplayModule GameplayPrefab { get; private set; }
}
