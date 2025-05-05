using UnityEngine;

[CreateAssetMenu(fileName = "NetModuleData", menuName = "Modules/Net Module Data")]
public class NetModuleData : ScriptableObject
{
    public NetModuleID moduleID;
    public int cost;
    public NetModuleBaseStats baseStats;

    public NetEditorModule shipEditorPrefab;
    public NetGameplayModule gameplayPrefab;
}
