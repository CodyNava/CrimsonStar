using FishNet;
using FishNet.Connection;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _target;
    [Header("References")]
    [SerializeField] private Camera cam;
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Vector3 offset;

    [Header("Manual Settings")] 
    [SerializeField] private float panSpeedFactor = 0.75f;
    [SerializeField] private float panBoostFactor = 3f;

    private Vector3 velocity = Vector3.zero;
    private NetGameplayConductor _gameplayConductor;

    private bool _isSpectatorMode = false;

    public float CameraDistance
    {
        get => offset.z;
        set => offset.z = value;
    }

    public void SetTarget(Transform target) => _target = target;

    public void Start()
    {
        if (InstanceFinder.TryGetInstance(out _gameplayConductor))
        {
            _gameplayConductor.OnLocalPlayerDeath += OnLocalPlayerDeath;
        }
    }

    private void OnLocalPlayerDeath(NetGameplayConductor.LocalPlayerDeathEventArgs arg0)
    {
        _isSpectatorMode = true;
        _target = null;
    }

    public void OnDisable()
    {
        if (!_gameplayConductor.IsUnityNull())
        {
            _gameplayConductor.OnLocalPlayerDeath -= OnLocalPlayerDeath;
        }
    }

    void LateUpdate()
    {
        if (!_target.IsUnityNull())
        {
            Vector3 targetPos = _target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

            if (_isSpectatorMode && Keybinds.Actions.Camera.CameraPan.ReadValue<Vector2>().sqrMagnitude > 0) _target = null;
        }
        else
        {
            Vector3 pos = transform.position;
            
            Vector2 input = Keybinds.Actions.Camera.CameraPan.ReadValue<Vector2>();
            float speed = panSpeedFactor * (Keybinds.Actions.Camera.CameraBoost.IsPressed() ? panBoostFactor : 1);
            input = input.normalized * speed;
            Vector3 targetPos = new (pos.x + input.x, pos.y + input.y, offset.z);
            transform.position = Vector3.SmoothDamp(pos, targetPos, ref velocity, smoothTime);
        }
    }
    public void SetTargetFollow(Transform target)
    {
        _target = target;
    }
}
