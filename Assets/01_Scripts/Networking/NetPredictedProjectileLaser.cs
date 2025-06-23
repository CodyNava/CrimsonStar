using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

public class NetPredictedProjectileLaser : MonoBehaviour
{
    [SerializeField] private float maxLength = 10f;
    [SerializeField] private float growSpeed = 2f;   
    [SerializeField] private float lifetimeAfterFullGrow = 2f;
    [SerializeField] private int maxHits = 3;
    [SerializeField] private float projectileDamage;
    [SerializeField] private VisualEffect bulletVFX;
    [SerializeField] private GameObject hitFeedbackVFX;

    private NetTeamID _netTeamID;
    private Vector3 _direction;
    
    private float _growProgress = 0f;
    private float _currentLength = 0f;
    private Vector3 _initialScale;

    private HashSet<NetGameplayModule> hitModules = new HashSet<NetGameplayModule>();
    
    private ulong _attackerID;
    private bool _fullyGrown = false;
    private float _lifetimeTimer = 0f;
    
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _direction = direction.normalized;
        _netTeamID = netTeamID;
        _attackerID = attackerID;

        _initialScale = transform.localScale;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);
        
        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        
        if (!_fullyGrown)
        {
            _growProgress += growSpeed * dt;
            _growProgress = Mathf.Clamp01(_growProgress);
            _currentLength = Mathf.Lerp(0f, maxLength, _growProgress);
            transform.localScale = new Vector3(_initialScale.x, _currentLength, _initialScale.z);

            if (_growProgress >= 1f)
            {
                _fullyGrown = true;
                _lifetimeTimer = lifetimeAfterFullGrow;
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
        if (!other.TryGetComponent(out NetGameplayModule module)) return;
        if (module.NetTeamID == _netTeamID) return;
        if (hitModules.Contains(module)) return;

        hitModules.Add(module);

        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            module.S_InflictDamage(projectileDamage, _attackerID);
        }
        
        if (InstanceFinder.IsClientStarted)
        {
            Instantiate(hitFeedbackVFX, other.transform.position, Quaternion.identity);
        }
        
        if (hitModules.Count >= maxHits)
        {
            Destroy(gameObject);
        }
    }

}
