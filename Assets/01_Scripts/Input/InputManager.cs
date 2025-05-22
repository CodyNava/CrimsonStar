using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SceneSingleton<InputManager>
{
    [SerializeField] private PlayerInput playerInput;

    public bool IsGamepadUsed => playerInput.currentControlScheme != Keybinds.Actions.KeyboardMouseScheme.name;

    protected override void Init()
    {
        playerInput.actions = Keybinds.Actions.asset;
    }

    public static void EnableGameControls()
    {
        Keybinds.Actions.Player.Enable();
        Keybinds.Actions.Camera.Enable();
    }

    public static void DisableGameControls()
    {
        Keybinds.Actions.Player.Disable();
        Keybinds.Actions.Camera.Disable();
    }
}
