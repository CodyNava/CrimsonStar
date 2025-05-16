using UnityEngine;
using UnityEngine.VFX;

public class ThrusterVFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect thrusterEffect;
    [SerializeField] private float changeSpeed = 2f;

    private float _currentStrength = 0f;
    private float _targetStrength = 0f;

    public void SetThrustActive(bool isThrusting)
    {
        _targetStrength = isThrusting ? 1f : 0f;
    }

    private void Update()
    {
        if (thrusterEffect == null) return;

        _currentStrength = Mathf.MoveTowards(_currentStrength, _targetStrength, changeSpeed * Time.deltaTime);
        thrusterEffect.SetFloat("ThrusterStrength", _currentStrength);
    }
}
