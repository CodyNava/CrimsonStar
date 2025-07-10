using FMODUnity;
using UnityEngine;

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
        float input = netMovementController.InputThrust;
        bool isThrusting = input > 0.2f;
        float targetStrenght = isThrusting ? 1f : 0f;
        _currentStrength = Mathf.MoveTowards(_currentStrength, targetStrenght, changeSpeed * Time.deltaTime);
        thrusterSound.SetParameter("Thrust", _currentStrength);
    }
}