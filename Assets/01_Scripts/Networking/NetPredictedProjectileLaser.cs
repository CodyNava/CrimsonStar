using System;
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

    private float _tickRate;
    private float _tickTimer;
    private int _maxTargetsPerTick;
    private float _growProgress = 0f;
    private float _currentLength = 0f;
    private float _initialWidth;
    
    private readonly Dictionary<NetGameplayModule, float> _damageTimers = new();
    
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
        turretTransform = spawnTransform;
        _tickRate = laserProjectileObject.LaserTickRate;
        _maxTargetsPerTick = Mathf.FloorToInt(laserProjectileObject.MaxTargetsPerHit);
        _initialWidth = laserProjectileObject.LaserWidth;
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
        LaserVisuels(dt);
        TickLifetime(dt);
        
        if (InstanceFinder.IsServerStarted)
            TickDamage(dt);
        if (InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted)
            TickDamage(dt);
    }
    
    

    private void LaserVisuels(float dt)
    {
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
    }
    
    private void TickLifetime(float dt)
    {
        if (!_fullyGrown) return;

        _lifetimeTimer -= dt;
        if (_lifetimeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }
    private void TickDamage(float dt)
    {
        _tickTimer += dt;
        if (_tickTimer < _tickRate) return;

        _tickTimer = 0f;

        int hits = 0;

        foreach (var module in _damageTimers.Keys)
        {
            if (hits >= _maxTargetsPerTick)
                break;

            if (module == null || module.Bridge == _bridgeOrigin)
                continue;

            if (!_lobbyConductor.IsUnityNull())
            {
                if (module.NetTeamID == _netTeamID && _lobbyConductor.FriendlyFireID == NetFirendlyFireID.Off)
                    continue;
            }

            float friendlyFireMult = 1f;
            if (module.NetTeamID == _netTeamID)
                friendlyFireMult = _lobbyConductor.FriendlyFireDamageMult;

            module.S_InflictDamage(laserProjectileObject.ProjectileDamage * friendlyFireMult, _attackerID);

            if (InstanceFinder.IsClientStarted)
            {
                var spawnPos = module.transform.position + new Vector3(0, 0, -2.5f);
                Instantiate(hitFeedbackVFX, spawnPos, Quaternion.identity);
                // vfx.transform.SetParent(other.transform);
            }
            
            hits++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out NetGameplayModule module))
        {
            _damageTimers.Remove(module);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.Bridge == _bridgeOrigin)
            return;

        if (_damageTimers.ContainsKey(module))
            return;
        
        if (!_lobbyConductor.IsUnityNull() &&
            module.NetTeamID == _netTeamID &&
            _lobbyConductor.FriendlyFireID == NetFirendlyFireID.Off)
            return;

        _damageTimers[module] = 0f;
        
    }

}
