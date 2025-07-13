using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPreset", menuName = "ColorPreset")]
public class  ColorPresetData : ScriptableObject
{
    [field: SerializeField] public List<string> presetNames;
    [field: SerializeField] public List<Color> presetColor1 = new List<Color>();
    [field: SerializeField] public List<Color> presetColor2 = new List<Color>();
    [field: SerializeField] public List<Color> presetColor3 = new List<Color>();
    [field: SerializeField] public List<Color> presetColor4 = new List<Color>();
    [field: SerializeField] public List<Color> presetColor5 = new List<Color>();
    [field: SerializeField] public List<Color> presetColor6 = new List<Color>();
}
