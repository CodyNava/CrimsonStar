using _01_Scripts.Projectiles;
using FishNet;
using Steamworks;
using UnityEngine;
using UnityEngine.VFX;

public class NetPredictedProjectile : MonoBehaviour
{
    [SerializeField] private VisualEffect bulletVFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] public BaseProjectileObject baseProjectileObject;
    
    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    private float _passedTime = 0f;
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _direction = direction;
        _passedTime = passedTime;
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        Destroy(gameObject, baseProjectileObject.ProjectileTimer);
        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction);
        }
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

        transform.position += _direction * (baseProjectileObject.ProjectileSpeed * (dt + passedDt));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.NetTeamID == _netTeamID) return;

        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            module.S_InflictDamage(baseProjectileObject.ProjectileDamage, _attackerID);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
