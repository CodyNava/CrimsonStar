using System;
using UnityEngine;

public class ModuleNavigation : MonoBehaviour
{
    private NetEditorModule _netModule;

    private void Start()
    {
        _netModule = GetComponentInParent<NetEditorModule>();
    }

    public void Update() => InvertRotation();
    
    private void InvertRotation()
    {
        var moduleTransform = _netModule.GetRotation();
        transform.localRotation = Quaternion.Inverse(moduleTransform.rotation);
    }
}
