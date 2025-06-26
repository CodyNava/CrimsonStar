using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

public class NetPredictedProjectileRocket : MonoBehaviour
{
    [SerializeField] private float projectileAcceleration;
    [SerializeField] private float projectileMaxSpeed;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileTimer;
    [SerializeField] private VisualEffect bulletVFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    
    
    public float ProjectileAcceleration=> projectileAcceleration;
    public float ProjectileMaxSpeed => projectileMaxSpeed;
    public float ProjectileDamage => projectileDamage;
    public float ProjectileTimer => projectileTimer;

    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    
    private Vector3 velocity = Vector3.zero;
    
    
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _direction = direction;
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        Destroy(gameObject, projectileTimer);
        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        Vector3 accVector = _direction.normalized * projectileAcceleration;
         velocity += accVector * dt;
         if (velocity.magnitude > projectileMaxSpeed)
             velocity = velocity.normalized * projectileMaxSpeed;

         transform.position += velocity * dt;
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
            module.S_InflictDamage(projectileDamage, _attackerID);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
