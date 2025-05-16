using System;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class VirtualMouseEvents : MonoBehaviour
{
    [SerializeField] private VirtualMouseInput virtualMouseInput;

    public static event Action inputEnabled;
    public static event Action inputDisabled;
    
    private static void OnInputEnabled()
    {
        inputEnabled?.Invoke();
    }
    
    private static void OnInputDisabled()
    {
        inputDisabled?.Invoke();
    }
}