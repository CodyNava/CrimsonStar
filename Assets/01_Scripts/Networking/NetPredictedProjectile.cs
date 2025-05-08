using FishNet;
using UnityEngine;

public class NetPredictedProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileDamage;

    private NetPlayerID _netPlayerID;
    private Vector3 _direction;
    private float _passedTime = 0f;
    
    public void Initialize(Vector3 direction, float passedTime, NetPlayerID netPlayerID)
    {
        _direction = direction;
        _passedTime = passedTime;
        _netPlayerID = netPlayerID;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        float passedDt = 0f;
        if (_passedTime > 0f)
        {
            float step = (_passedTime * 0.08f);
            _passedTime -= step;

            if (_passedTime <= (dt / 2f))
            {
                step += _passedTime;
                _passedTime = 0f;
            }

            passedDt = step;
        }
        
        transform.position += _direction * (projectileSpeed * (dt + passedDt));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.NetPlayerID == _netPlayerID) return;
        
        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            module.InflictDamage(projectileDamage);
        }
        
        Destroy(gameObject);
    }
}
