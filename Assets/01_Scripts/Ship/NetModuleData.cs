using UnityEngine;

[CreateAssetMenu(fileName = "ModuleData", menuName = "Module Data")]
public class ModuleData : ScriptableObject
{
    public NetModuleID moduleID;
    public int cost;
    public NetModuleBaseStats baseStats;

    public NetEditorModule shipEditorPrefab;
    public NetGameplayModule gameplayPrefab;
}
