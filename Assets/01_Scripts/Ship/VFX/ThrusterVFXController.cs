using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ThrusterVFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect thrusterEffect;
    [SerializeField] private InputActionReference thrustInput;
    [SerializeField] private float changeSpeed = 2f;
    private float _currentStrength = 0f;
    void Update()
    {
        SetThrusterStrenghtIfInput();
    }
    public void SetThrusterStrenghtIfInput()
    {
        if (thrusterEffect == null)
            return;
        Vector2 input = thrustInput.action.ReadValue<Vector2>();
        bool isThrusting = input.y > 0.2f;
        float targetStrength = isThrusting ? 1f : 0f;
        _currentStrength = Mathf.MoveTowards(_currentStrength, targetStrength, changeSpeed * Time.deltaTime);
        thrusterEffect.SetFloat("ThrusterStrength", _currentStrength);
       
    }
}
