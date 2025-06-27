using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;
using _01_Scripts.Projectiles;

public class NetPredictedProjectileRocket : MonoBehaviour
{
    [SerializeField] private VisualEffect bulletVFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] public RocketProjectileObject rocketProjectileObject;


    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    
    private Vector3 velocity = Vector3.zero;
    
    
    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _direction = direction;
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        Destroy(gameObject, rocketProjectileObject.ProjectileTimer);
        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        Vector3 accVector = _direction.normalized * rocketProjectileObject.ProjectileAcceleration;
         velocity += accVector * dt;
         if (velocity.magnitude > rocketProjectileObject.ProjectileMaxSpeed)
             velocity = velocity.normalized * rocketProjectileObject.ProjectileMaxSpeed;

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
            // module.S_InflictDamage(rocketProjectileObject.ProjectileDamage, _attackerID);
            // Spawn Explosion here
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
