using _01_Scripts.Projectiles;
using FishNet;
using Steamworks;
using Unity.VisualScripting;
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
    private NetBridge _bridgeOrigin;

    private NetLobbyConductor _lobbyConductor;
    
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID, NetBridge bridgeOrigin)
    {
        _direction = direction;
        _passedTime = passedTime;
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        _bridgeOrigin = bridgeOrigin;
        Destroy(gameObject, baseProjectileObject.ProjectileTimer);
        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction);
        }

        InstanceFinder.TryGetInstance(out _lobbyConductor);
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
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.Bridge == _bridgeOrigin) return;

        if (!_lobbyConductor.IsUnityNull())
        {
            if (module.NetTeamID == _netTeamID && _lobbyConductor.FriendlyFireID == NetFirendlyFireID.Off) return;
        }

        if (InstanceFinder.IsClientStarted)
        {
            Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        }

        if (InstanceFinder.IsServerStarted)
        {
            float friendlyFireDamageMult = 1f;
            if (module.NetTeamID == _netTeamID) friendlyFireDamageMult = _lobbyConductor.FriendlyFireDamageMult;
            
            module.S_InflictDamage(baseProjectileObject.ProjectileDamage * friendlyFireDamageMult, _attackerID);
        }
        
        Destroy(gameObject);
    }
}
