using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "NetModuleDB", menuName = "Modules/Net Module DB")]
public class NetModuleDB : ScriptableObject
{
    [SerializedDictionary("Module ID", "Module Data")]
    [field: SerializeField] public SerializedDictionary<NetModuleID, NetModuleData> ModuleData { get; private set; }
}