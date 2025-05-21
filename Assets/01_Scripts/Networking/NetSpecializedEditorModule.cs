using UnityEngine;

/// <summary>
/// Base class for NetEditorModules which must provide access to a scriptable object specific to that module
/// </summary>
/// <typeparam name="T">The type of scriptable object related to this module.</typeparam>
public class NetSpecializedEditorModule<T> : NetEditorModule where T : ScriptableObject
{
    [field: SerializeField] public T ModuleScriptableObject { get; private set; }
}