using UnityEngine;

public class DataProvider : SceneSingleton<DataProvider>
{
    [field: SerializeField] public NetModuleDB ModuleDB { get; private set; }
    [field: SerializeField] public NetShipEditorConfig DefaultEditorResources { get; private set; }
}
