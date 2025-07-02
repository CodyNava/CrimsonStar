using System;
using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

public class NetPredictedExplosion : MonoBehaviour
{
    [SerializeField] public ExplosionObject explosionObject;
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] private GameObject ScalingObject;

    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    private Vector3 startScale;
    private Vector3 endScale;
    private float timer;
    
    private HashSet<NetGameplayModule> hitModules = new HashSet<NetGameplayModule>();
    
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        
        Destroy(gameObject, explosionObject.ExplosionTimer);
        if (VFX.HasVector3("DirectionVector_position"))
        {
            VFX.SetVector3("DirectionVector_position", _direction);
        }
    }

    private void Start()
    {
        Destroy(gameObject, explosionObject.ExplosionTimer);
        startScale = Vector3.one * explosionObject.ExplosionMinSize;
        endScale = Vector3.one * explosionObject.ExplosionMaxSize;
        ScalingObject.transform.localScale = startScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / explosionObject.ExplosionTimer);
        ScalingObject.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
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
            module.S_InflictDamage(explosionObject.ExplosionDamage, _attackerID);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
    }

}
