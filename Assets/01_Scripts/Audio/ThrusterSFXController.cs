using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrusterSFXController : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter thrusterSound;
    [SerializeField] private NetMovementController netMovementController;
    [SerializeField] private float changeSpeed = 2f;
    private float _currentStrength;

    private void Update()
    {
        SetThrusterSFXStrenght();
    }

    private void SetThrusterSFXStrenght()
    {
        bool isGamepad = Gamepad.current != null && Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.01f;

        float thrustCheck = 0f;
        if (isGamepad)
        {
            Vector2 gamepadInput = Gamepad.current.leftStick.ReadValue();
            thrustCheck = gamepadInput.magnitude;
        }
        else
        {
            thrustCheck = netMovementController.InputThrust;
        }
        bool isThrusting = thrustCheck > 0.01f;
        float targetStrenght = isThrusting ? 1f : 0f;
        _currentStrength = Mathf.MoveTowards(_currentStrength, targetStrenght, changeSpeed * Time.deltaTime);
        thrusterSound.SetParameter("Thrust", _currentStrength);
    }
}