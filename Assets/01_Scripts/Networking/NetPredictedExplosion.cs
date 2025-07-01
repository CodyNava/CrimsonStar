using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

public class NetPredictedExplosion : MonoBehaviour
{
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionTimer;
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] private float explosionGrowSpeed;
    
    public float ExplosionGrowSpeed => explosionGrowSpeed;
    public float ExplosionDamage => explosionDamage;
    public float ExplosionTimer => explosionTimer;

    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    
    private Vector3 size = Vector3.zero;
    
    private HashSet<NetGameplayModule> hitModules = new HashSet<NetGameplayModule>();
    
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        Destroy(gameObject, explosionTimer);
        if (VFX.HasVector3("DirectionVector_position"))
        {
            VFX.SetVector3("DirectionVector_position", _direction);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        transform.localScale += size * (dt * explosionGrowSpeed);
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
            module.S_InflictDamage(explosionDamage, _attackerID);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
    }

}
