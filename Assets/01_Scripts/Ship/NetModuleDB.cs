using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "NetModuleDB", menuName = "Modules/Net Module DB")]
public class NetModuleDB : ScriptableObject
{
    [SerializedDictionary("Module ID", "Module Data")]
    public SerializedDictionary<NetModuleID, NetModuleData> moduleData;
}