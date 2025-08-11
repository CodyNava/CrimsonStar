using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPresetButton : MonoBehaviour
{
    [SerializeField] private Image color1;
    [SerializeField] private Image color2;
    [SerializeField] private Image color3;
    [SerializeField] private TMP_Text presetName;

    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite painterBut1, painterBut2, painterBut3;


    public static event Action<string> ColorSelected;

    public void InvokePreset()
    {
        ColorSelected?.Invoke(presetName.text);
    }

    public void SetColor(ColorPreset data) => (color1.color, color2.color, color3.color) = data;


    public void SetButtonSprite()
    {
        buttonImage.sprite = painterBut1;
    }

    public void ChangeButtonSpriteBasedOnState() => buttonImage.sprite = buttonImage.sprite == painterBut2
        ? painterBut1
        : painterBut2;

    public void SetName(string name)
    {
        presetName.text = name;
    }
}