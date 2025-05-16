using UnityEngine;
using UnityEngine.InputSystem.UI;

public class VirtualMouseEvents : MonoBehaviour
{
    [SerializeField] private VirtualMouseInput virtualMouseInput;

    private void OnEnable()
    {
        InputManager.GameInputEnabled -= OnGameInputEnabled;
        InputManager.GameInputDisabled -= OnGameInputDisabled;
        InputManager.GameInputDisabled += OnGameInputDisabled;
        InputManager.GameInputEnabled += OnGameInputEnabled;
    }

    private void OnGameInputDisabled()
    {
        virtualMouseInput.enabled = true;
    }

    private void OnGameInputEnabled()
    {
        virtualMouseInput.enabled = false;
    }

    private void OnDisable()
    {
        InputManager.GameInputEnabled -= OnGameInputEnabled;
        InputManager.GameInputDisabled -= OnGameInputDisabled;
    }
}