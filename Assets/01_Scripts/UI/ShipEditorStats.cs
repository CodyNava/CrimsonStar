using System.Collections.Generic;
using System.Linq;
using FishNet;
using TMPro;
using UnityEngine;

public class ShipEditorStats : MonoBehaviour
{
    [Header("totalStats")] [SerializeField]
    private TMP_Text totalHealth;

    [SerializeField] private TMP_Text hexCount;
    private float _totalHealth;
    private float _hexCount;

    [Header("Weapon")] [SerializeField] private TMP_Text damagePerSecond;
    [SerializeField] private TMP_Text maxRange;
    [SerializeField] private TMP_Text minRange;
    private float _damagePerSecond;
    private float _maxRange;
    private float _minRange;
    private float _rocketRange;
    private float _turretRange;
    private float _laserRange;

    [Header("Speed")] [SerializeField] private TMP_Text maxSpeed;
    [SerializeField] private TMP_Text acceleration;
    [SerializeField] private TMP_Text maneuverability;
    private float _maxSpeed;
    private float _mass;
    private float _thrust;
    private float _acceleration;
    private float _maneuverability;

    [SerializeField] private NetBridgeConfig netBridge;

    public void GetTotalStats(IReadOnlyList<NetEditorModule> moduleList)
    {
        Debug.Log(moduleList.Count);
        _totalHealth = 0;
        _hexCount = 0;
        _damagePerSecond = 0;
        _maxSpeed = 0;
        _mass = 0;
        _thrust = 0;
        _acceleration = 0;
        _maneuverability = 0;
        _rocketRange = 0;
        _laserRange = 0;
        _turretRange = 0;
        _maxRange = 0;
        _minRange = 0;

        foreach (NetEditorModule modules in moduleList)
        {
            _totalHealth += modules.ModuleData.BaseStats.health;
            _hexCount += modules.ModuleData.HexagonSize;
            _mass += modules.ModuleData.BaseStats.mass;
            _thrust += modules.ModuleData.BaseStats.thrust;
            _maxSpeed = netBridge.MaxMovementSpeed / (1 + _mass);
            _acceleration = netBridge.BaseMovementSpeed + _thrust / (1 + _mass);
            _maneuverability = netBridge.MaxAngularSpeed / (1 + _mass);

            if (modules is NetTurretEditorModule turretModule)
            {
                var turretProjectile = turretModule.ModuleScriptableObject.Projectile.baseProjectileObject;
                
                float projDmg = turretProjectile.ProjectileDamage;
                float shootingCd = turretModule.ModuleScriptableObject.Cooldown;
                float projTimer = turretProjectile.ProjectileTimer;
                float projSpeed = turretProjectile.ProjectileSpeed;
                
                _damagePerSecond += projDmg / shootingCd;
                _turretRange = projSpeed * projTimer;
            }
            
            if (modules is NetLaserTurretEditorModule laserTurretModule)
            {
                var laserProjectile = laserTurretModule.ModuleScriptableObject.Projectile.laserProjectileObject;
                
                float projDmg = laserProjectile.ProjectileDamage;
                float shootingCd = laserTurretModule.ModuleScriptableObject.Cooldown + laserTurretModule.ModuleScriptableObject.ChargeTime;
                float maxHits = laserProjectile.MaxHits;
                float maxRangeLaser = laserProjectile.MaxLength;
                
                _damagePerSecond += (projDmg * maxHits) / shootingCd;
                _laserRange = maxRangeLaser;
            }
            
            if (modules is NetRocketEditorModule rocketEditorModule)
            {
                //float explDmg = rocketEditorModule.ModuleScriptableObject.Projectile.rocketProjectileObject.   //todo explosion damage beachten
                var rocketProjectile = rocketEditorModule.ModuleScriptableObject.Projectile.rocketProjectileObject;
                
                float projDmg = rocketProjectile.ProjectileDamage;
                float shootingCd = rocketEditorModule.ModuleScriptableObject.Cooldown; 
                float rocketTimer = rocketProjectile.ProjectileTimer;
                float dt = (float)InstanceFinder.TimeManager.TickDelta;
                int numberOfTicks = Mathf.FloorToInt(rocketTimer / dt);
                float distanceTraveled = 0f;
                float velocity = 0f;
                float rocketSpeed = rocketProjectile.ProjectileAcceleration;
                for (int i = 0; i < numberOfTicks; i++)
                {
                    velocity += rocketSpeed * dt;
                    distanceTraveled += velocity * dt;
                }
                
                float rocketRangeUnrounded = distanceTraveled / 10;
                _damagePerSecond += projDmg / shootingCd;
                _rocketRange = Mathf.RoundToInt(rocketRangeUnrounded) * 10;
                
            }
            
            List<float> validValues = new List<float> { _rocketRange, _turretRange, _laserRange }.Where(v => v != 0f).ToList();
            _maxRange = validValues.Count > 0 ? validValues.Max() : 0f;
            _minRange = validValues.Count > 0 ? validValues.Min() : 0f;
        }
        
        DisplayStats();
        // todo: make the speed calcuations more clean by getting the value directly from NetBridge
    }

    public void DisplayStats()
    {
        totalHealth.text = $"TotalHealth: {_totalHealth:0}";
        hexCount.text = $"HexCount: {_hexCount:0}";
        maxSpeed.text = $"MaxSpeed: {_maxSpeed:0.0}";
        acceleration.text = $"Acceleration: {_acceleration:0.0}";
        maneuverability.text = $"Maneuverability: {_maneuverability:0.0}";
        damagePerSecond.text = $"DmgPerSecond: {_damagePerSecond:0}";
        maxRange.text = $"MaxRange: {_maxRange:0}";
        minRange.text = $"MinRange: {_minRange:0}";
    }
}