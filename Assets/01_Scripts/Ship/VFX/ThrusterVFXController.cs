using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ThrusterVFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect thrusterEffect;
    [SerializeField] private InputActionReference thrustInput;
    [SerializeField] private float changeSpeed = 2f;
    private float _currentStrength = 0f;
    private float _targetStrength = 0f;
    void Update()
    {
        SetThrusterStrenghtIfInput();
    }
    public void SetThrusterStrenghtIfInput()
    {
        if (thrusterEffect == null) return;

        _currentStrength = Mathf.MoveTowards(_currentStrength, _targetStrength, changeSpeed * Time.deltaTime);
        thrusterEffect.SetFloat("ThrusterStrength", _currentStrength);
    }
    public void UpdateThrustVisual(Vector2 input)
    {
        _targetStrength = input.y > 0.2f ? 1f : 0f;
    }
}
