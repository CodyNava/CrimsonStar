using System;
using System.Runtime.ConstrainedExecution;
using FishNet;
using UnityEngine;
using UnityEngine.VFX;
using _01_Scripts.Projectiles;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;


public class NetPredictedProjectileRocket : NetworkBehaviour
{
    [SerializeField] private VisualEffect bulletVFX;
    [SerializeField] private GameObject hitFeedbackVFX;
    [SerializeField] public RocketProjectileObject rocketProjectileObject;
    [SerializeField] private NetGameplayModule turretModule;


    private const float MaxPassedTime = 0.3f;
    private ulong _attackerID;
    private readonly SyncVar<NetTeamID> _netTeamID = new SyncVar<NetTeamID>();
    private readonly SyncVar<Vector3> _direction = new SyncVar<Vector3>();
    private Vector3 velocity = Vector3.zero;
    private float lifeTime;
    private bool _hasShot;


    public void Initialize(Vector3 direction, float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _direction.Value = direction;
        _netTeamID.Value = netTeamID;
        _attackerID = attackerID;


        if (bulletVFX.HasVector3("DirectionVector_position"))
        {
            bulletVFX.SetVector3("DirectionVector_position", _direction.Value);
        }
    }

    private void Start()
    {
        lifeTime = rocketProjectileObject.ProjectileTimer;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        Vector3 accVector = _direction.Value.normalized * rocketProjectileObject.ProjectileAcceleration;
        velocity += accVector * dt;
        if (velocity.magnitude > rocketProjectileObject.ProjectileMaxSpeed)
            velocity = velocity.normalized * rocketProjectileObject.ProjectileMaxSpeed;

        transform.position += velocity * dt;
        lifeTime -= dt;
        if (lifeTime <= 0f && !_hasShot)
        {
            _hasShot = true;
            if (InstanceFinder.IsServerStarted)
            {
                S_SpawnExplosion(transform.position, _attackerID);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) ||
            module.NetTeamID == _netTeamID.Value) return;
        if (_hasShot) return;

        _hasShot = true;

        if (InstanceFinder.IsClientStarted)
        {
            Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        }

        if (InstanceFinder.IsServerStarted)
        {
            S_SpawnExplosion(transform.position, _attackerID);
        }

        Destroy(gameObject);
    }

    [Server]
    private void S_SpawnExplosion(Vector3 pos, ulong senderID)
    {
       // float passedTime = (float)TimeManager.TimePassed(tick, false);
       // passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        C_SpawnExplosion(pos, senderID);
        C_ObserversSpawnExplosion(pos, senderID, Channel.Reliable);
    }

    [ObserversRpc]
    private void C_ObserversSpawnExplosion(Vector3 pos,  ulong senderID, Channel channel = Channel.Reliable)
    {
      //  float passedTime = (float)TimeManager.TimePassed(tick, false);
       // passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        C_SpawnExplosion(pos, senderID);
    }

    private void C_SpawnExplosion(Vector3 position, ulong senderID)
    {
        print("Spawning Explosion");
        NetPredictedExplosion pe = Instantiate(rocketProjectileObject.Explosion, position, Quaternion.identity);
        pe.Initialize(_netTeamID.Value, senderID);
    }
}