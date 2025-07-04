using System;
using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;
using _01_Scripts.Projectiles;


public class NetPredictedExplosion : MonoBehaviour
{
    private ExplosionObject _explosionObject;
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] private GameObject ScalingObject;


    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    private Vector3 startScale;
    private Vector3 endScale;
    private float timer;
    private CircleCollider2D circleCollider;
    
    private HashSet<NetGameplayModule> hitModules = new HashSet<NetGameplayModule>();
    
    public void Initialize(NetTeamID netTeamID, ulong attackerID, ExplosionObject explosionObject)
    {
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        _explosionObject = explosionObject;
        
        Destroy(gameObject, explosionObject.ExplosionTimer);
        if (VFX.HasVector3("DirectionVector_position"))
        {
            VFX.SetVector3("DirectionVector_position", _direction);
        }
    }

    private void Start()
    {
        startScale = Vector3.one * _explosionObject.ExplosionMinSize;
        endScale = Vector3.one * _explosionObject.ExplosionMaxSize;
        ScalingObject.transform.localScale = startScale;
        
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = _explosionObject.ExplosionMinSize / 2f;
        
        Destroy(gameObject, _explosionObject.ExplosionTimer);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / _explosionObject.ExplosionTimer);
        Vector3 currentScale = Vector3.Lerp(startScale, endScale, progress);
        
        ScalingObject.transform.localScale = currentScale;
        
        circleCollider.radius = currentScale.x / 2f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.NetTeamID == _netTeamID) return;
        if (hitModules.Contains(module)) return;
        
        hitModules.Add(module);
        
        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            module.S_InflictDamage(_explosionObject.ExplosionDamage, _attackerID);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
    }

}
