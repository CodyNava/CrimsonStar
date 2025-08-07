using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;
using _01_Scripts.Projectiles;
using Unity.VisualScripting;

public class NetPredictedProjectileLaser : MonoBehaviour
{
    [SerializeField] private VisualEffect bulletVFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] public GameObject hitBox;
    [SerializeField] public LaserProjectileObject laserProjectileObject;
    [SerializeField] public Transform turretTransform;
    
    

    private NetTeamID _netTeamID;
    private Vector3 _direction;
    private Vector3 _endPoint;
    private NetBridge _bridgeOrigin;
    private NetLobbyConductor _lobbyConductor;
    
    private float _growProgress = 0f;
    private float _currentLength = 0f;
    private float _initialWidth;

    private readonly HashSet<NetGameplayModule> _hitModules = new HashSet<NetGameplayModule>();
    
    private ulong _attackerID;
    private bool _fullyGrown = false;
    private float _lifetimeTimer = 0f;
    
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID, NetBridge bridgeOrigin, Transform spawnTransform)
    {
        bulletVFX.Play();
        _direction = direction.normalized;
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        _bridgeOrigin = bridgeOrigin;
        _initialWidth = laserProjectileObject.LaserWidth;
        turretTransform = spawnTransform;
        _endPoint = turretTransform.position + (_direction * laserProjectileObject.MaxLength);
        
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);
        
        //int y = bulletVFX.GetInt("y_asd");
        //y = (int)_currentLength;
        
        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction);
        }
        
        InstanceFinder.TryGetInstance(out _lobbyConductor);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        
        //laser locking
        Vector3 startPos = turretTransform.position;
        Vector3 direction = _endPoint - startPos;
        float length = direction.magnitude;
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        //
        if (!_fullyGrown)
        {
            _growProgress += laserProjectileObject.GrowSpeed * dt;
            _growProgress = Mathf.Clamp01(_growProgress);
            _currentLength = Mathf.Lerp(0f, length, _growProgress);
            //transform.localScale = new Vector3(_initialScale.x, _currentLength, _initialScale.z);
            hitBox.transform.localScale = new Vector2(_initialWidth, _currentLength);

            if (_growProgress >= 1f)
            {
                _fullyGrown = true;
                _lifetimeTimer = laserProjectileObject.LifetimeAfterFullGrown;
            }
        }
        else
        {
            _lifetimeTimer -= dt;
            if (_lifetimeTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.Bridge == _bridgeOrigin) return;
        if (_hitModules.Contains(module)) return;

        if (!_lobbyConductor.IsUnityNull())
        {
            if (module.NetTeamID == _netTeamID && _lobbyConductor.FriendlyFireID == NetFirendlyFireID.Off) return;
        }


        _hitModules.Add(module);
        
        if (InstanceFinder.IsServerStarted)
        {
            float friendlyFireDamageMult = 1f;
            if (module.NetTeamID == _netTeamID) friendlyFireDamageMult = _lobbyConductor.FriendlyFireDamageMult;
            
            module.S_InflictDamage(laserProjectileObject.ProjectileDamage * friendlyFireDamageMult, _attackerID);
        }
        
        if (InstanceFinder.IsClientStarted)
        {
            var spawnPos = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z - 2.5f);
            Instantiate(hitFeedbackVFX, spawnPos, Quaternion.identity);
            hitFeedbackVFX.transform.SetParent(other.transform);
        }
        
        if (_hitModules.Count >= laserProjectileObject.MaxHits)
        {
            _fullyGrown = true;
            _lifetimeTimer = laserProjectileObject.LifetimeAfterFullGrown;
            hitBox.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

}
