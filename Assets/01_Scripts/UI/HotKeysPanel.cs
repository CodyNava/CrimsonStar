using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotKeysPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text placeButton, sellButton, rotateLButton, rotateRButton;

    [SerializeField] private Sprite placeGp, sellGp, placeKm, sellKm;
    [SerializeField] private Image placeImage, sellImage;
    [SerializeField] private Color placeGpColor, sellGpColor, basicColor;


    private void Update()
    {
        SetHotKeyBasedOnDevice();
    }

    private void SetHotKeyBasedOnDevice()
    {
        placeButton.text = InputManager.Instance.IsGamepadUsed ? "A" : "LMB";
        sellButton.text = InputManager.Instance.IsGamepadUsed ? "B" : "RMB";
        rotateLButton.text = InputManager.Instance.IsGamepadUsed ? "LT" : "Q";
        rotateRButton.text = InputManager.Instance.IsGamepadUsed ? "RT" : "E";
        
        rotateRButton.text = InputManager.Instance.IsGamepadUsed ? "RT" : "E";
        rotateRButton.text = InputManager.Instance.IsGamepadUsed ? "RT" : "E";
        
        placeImage.sprite = InputManager.Instance.IsGamepadUsed ? placeGp : placeKm;
        sellImage.sprite = InputManager.Instance.IsGamepadUsed ? sellGp : sellKm;
        
        placeImage.color = InputManager.Instance.IsGamepadUsed ? placeGpColor : basicColor;
        sellImage.color = InputManager.Instance.IsGamepadUsed ? sellGpColor : basicColor;
    }
}