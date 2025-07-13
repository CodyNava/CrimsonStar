using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPresetButton : MonoBehaviour
{
    [SerializeField] private Image colorOne;
    [SerializeField] private Image colorTwo;
    [SerializeField] private Image colorThree;
    [SerializeField] private TMP_Text presetName;

    [SerializeField] private List<Color> currentPreset = new List<Color>();

    public IReadOnlyList<Color> ChangeColor() => currentPreset;
    public static event Action<List<Color>> ColorSelected;

    public void InvokePreset()
    {
        ColorSelected?.Invoke(currentPreset);
    }
    public void SetColor(List<Color> data)
    {
            colorOne.color = data[0];
            colorTwo.color = data[1];
            colorThree.color = data[2];
            currentPreset = data;
            
    }
    public void SetName(ColorPresetData data ,int nameindex)
    {
        presetName.text = data.presetNames[nameindex];
    }
}