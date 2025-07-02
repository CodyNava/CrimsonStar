using System;
using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using _01_Scripts.Projectiles;
using FishNet.Object;


public class NetPredictedProjectileRocket : NetworkBehaviour
{
    [SerializeField] private VisualEffect bulletVFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] public RocketProjectileObject rocketProjectileObject;
    [SerializeField] private NetGameplayModule turretModule;


    private const float MaxPassedTime = 0.3f;
    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    private Vector3 velocity = Vector3.zero;
    private float lifeTime;
    private bool _hasShot;


    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _direction = direction;
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        
        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction);
        }
    }

    private void Start()
    {
        lifeTime = rocketProjectileObject.ProjectileTimer;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        Vector3 accVector = _direction.normalized * rocketProjectileObject.ProjectileAcceleration;
        velocity += accVector * dt;
        if (velocity.magnitude > rocketProjectileObject.ProjectileMaxSpeed)
            velocity = velocity.normalized * rocketProjectileObject.ProjectileMaxSpeed;

        transform.position += velocity * dt;
        lifeTime -= dt;
        if (lifeTime <= 0f && !_hasShot)
        {
            _hasShot = true;
            S_SpawnExplosion(transform.position, _direction, TimeManager.Tick, _attackerID);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.NetTeamID == _netTeamID) return;
        if (_hasShot) return;
        
        _hasShot = true;
        
        if (InstanceFinder.IsClientStarted)
        {
            Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        }

        if (InstanceFinder.IsServerStarted)
        {
            S_SpawnExplosion(transform.position, _direction, TimeManager.Tick, _attackerID);
        }
        
        Destroy(gameObject);
    }
    
    [Server]
    private void S_SpawnExplosion(Vector3 pos, Vector3 dir, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        C_SpawnExplosion(pos, dir, passedTime, senderID);
        C_ObserversSpawnExplosion(pos, dir, tick, senderID);
    }
    
    [ObserversRpc]
    private void C_ObserversSpawnExplosion(Vector3 pos, Vector3 dir, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        C_SpawnExplosion(pos, dir, passedTime, senderID);
    }
    
    private void C_SpawnExplosion(Vector3 position, Vector3 direction, float passedTime, ulong senderID)
    {
        print("Spawning Explosion");
        NetPredictedExplosion pe = Instantiate(rocketProjectileObject.Explosion, position, Quaternion.identity);
        pe.Initialize(direction, passedTime, _netTeamID, senderID);
    }
    
}