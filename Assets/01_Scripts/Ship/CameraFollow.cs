using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CameraFollow : MonoBehaviour
{
    private NetBridge _target;
    public NetBridge Target => _target;
    public Transform TargetTransform => _target.VisualRootTransform;
    [Header("References")]
    [SerializeField] private Camera cam;
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Vector3 offset;
    public Vector3 OffSet => offset;

    [Header("Manual Settings")] 
    [SerializeField] private float panSpeedFactor = 0.75f;
    [SerializeField] private float panBoostFactor = 3f;

    private Vector3 velocity = Vector3.zero;
    private NetGameplayConductor _gameplayConductor;

    private bool _isSpectatorMode = false;


    public event UnityAction<NetBridge> OnTargetChanged; 

    public float CameraDistance
    {
        get => offset.z;
        set => offset.z = value;
    }

    public void SetTarget(NetBridge target)
    {
        _target = target;
        OnTargetChanged?.Invoke(_target);
    }

    public void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<NetGameplayBroadcasts.PlayerSpactate>(OnSpectatorBroadcast);
        InstanceFinder.ClientManager.RegisterBroadcast<NetGameplayBroadcasts.PlayerDeath>(OnPlayerDeath);
    }

    private void OnPlayerDeath(NetGameplayBroadcasts.PlayerDeath msg, Channel channel)
    {
        if (msg.conn != InstanceFinder.ClientManager.Connection) return;
        EnableSpectator();
    }

    private void OnSpectatorBroadcast(NetGameplayBroadcasts.PlayerSpactate msg, Channel channel)
    {
        EnableSpectator();
    }
    
    private void EnableSpectator()
    {
        _isSpectatorMode = true;
        _target = null;
    }

    public void OnDisable()
    {
        if (InstanceFinder.ClientManager != null)
        {
            InstanceFinder.ClientManager.UnregisterBroadcast<NetGameplayBroadcasts.PlayerSpactate>(OnSpectatorBroadcast);
            InstanceFinder.ClientManager.UnregisterBroadcast<NetGameplayBroadcasts.PlayerDeath>(OnPlayerDeath);
        }
    }

    void LateUpdate()
    {
        if (!_target.IsUnityNull())
        {
            Vector3 targetPos = TargetTransform.position + offset;
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
}
