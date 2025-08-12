using System;
using TMPro;
using UnityEngine;

public class VersionNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _versionText;
    
    private void Awake()
    {
        _versionText.text = "Version: " + Application.version;
    }
}
