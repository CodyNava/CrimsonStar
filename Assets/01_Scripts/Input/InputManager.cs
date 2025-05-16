using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SceneSingleton<InputManager>
{
    [SerializeField] private PlayerInput playerInput;
    
    
    public static event Action GameInputEnabled;
    public static event Action GameInputDisabled;
    

    public bool IsGamepadUsed => playerInput.currentControlScheme != Keybinds.Actions.KeyboardMouseScheme.name;

    protected override void Init()
    {
        playerInput.actions = Keybinds.Actions.asset;
    }

    public static void EnableGameControls()
    {
        Keybinds.Actions.Player.Enable();
        GameInputEnabled?.Invoke();
    }

    public static void DisableGameControls()
    {
        Keybinds.Actions.Player.Disable();
        GameInputDisabled?.Invoke();
    }
}
