using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship Editor Config", menuName = "Ship Editor/Ship Editor Config", order = 0)]
public class NetShipEditorConfig : ScriptableObject
{
    [SerializedDictionary("Resource Type", "Default Resource Count")]
    [field: SerializeField] public SerializedDictionary<NetResourceType, int> DefaultResourceCounts { get; private set; }
}
