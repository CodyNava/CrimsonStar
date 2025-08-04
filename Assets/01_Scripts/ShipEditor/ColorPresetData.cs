using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPresetData", menuName = "Color Preset Data")]
public class ColorPresetData : ScriptableObject
{
    public List<ColorPreset> presets = new List<ColorPreset>();
}