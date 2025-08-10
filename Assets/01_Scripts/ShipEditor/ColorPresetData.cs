using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPresetData", menuName = "Color Preset Data")]
public class ColorPresetData : ScriptableObject
{
    [SerializedDictionary("name", "colors")]
    public SerializedDictionary<string, ColorPreset> presets;
    
}