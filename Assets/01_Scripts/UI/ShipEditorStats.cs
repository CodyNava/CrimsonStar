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

    [Header("WeaponGroup1")]
    [SerializeField] private TMP_Text damagePerSecondWg1;
    [SerializeField] private TMP_Text maxRangeWg1;
    [SerializeField] private TMP_Text minRangeWg1;
    private float _damagePerSecondWg1;
    private float _maxRangeWg1;
    private float _minRangeWg1;
    private float _rocketRangeWg1;
    private float _turretRangeWg1;
    private float _laserRangeWg1;
    
    [Header("WeaponGroup2")]
    [SerializeField] private TMP_Text damagePerSecondWg2;
    [SerializeField] private TMP_Text maxRangeWg2;
    [SerializeField] private TMP_Text minRangeWg2;
    private float _damagePerSecondWg2;
    private float _maxRangeWg2;
    private float _minRangeWg2;
    private float _rocketRangeWg2;
    private float _turretRangeWg2;
    private float _laserRangeWg2;
    
    [Header("WeaponGroup3")]
    [SerializeField] private TMP_Text damagePerSecondWg3;
    [SerializeField] private TMP_Text maxRangeWg3;
    [SerializeField] private TMP_Text minRangeWg3;
    private float _damagePerSecondWg3;
    private float _maxRangeWg3;
    private float _minRangeWg3;
    private float _rocketRangeWg3;
    private float _turretRangeWg3;
    private float _laserRangeWg3;

    [Header("Speed")] [SerializeField] private TMP_Text maxSpeed;
    [SerializeField] private TMP_Text acceleration;
    [SerializeField] private TMP_Text maneuverability;
    private float _maxSpeed;
    private float _mass;
    private float _thrust;
    private float _acceleration;
    private float _maneuverability;

    [SerializeField] private NetBridgeConfig netBridge;

    public void GetTotalStats(IReadOnlyList<NetEditorModule> moduleList, ShipEditorWeaponGroups groupManager)
    {
        Debug.Log(moduleList.Count);

        var weaponGroupListOne = groupManager.weaponGroupOne;
        var weaponGroupListTwo = groupManager.weaponGroupTwo;
        var weaponGroupListThree = groupManager.weaponGroupThree;
        //All
        _totalHealth = 0;
        _hexCount = 0;
        _maxSpeed = 0;
        _mass = 0;
        _thrust = 0;
        _acceleration = 0;
        _maneuverability = 0;
        //Wg1
        _damagePerSecondWg1 = 0;
        _maxRangeWg1 = 0;
        _minRangeWg1 = 0;
        _rocketRangeWg1 = 0;
        _turretRangeWg1 = 0;
        _laserRangeWg1 = 0;
        //Wg2
        _damagePerSecondWg2 = 0;
        _maxRangeWg2 = 0;
        _minRangeWg2 = 0;
        _rocketRangeWg2 = 0;
        _turretRangeWg2 = 0;
        _laserRangeWg2 = 0;
        //Wg3
        _damagePerSecondWg3 = 0;
        _maxRangeWg3 = 0;
        _minRangeWg3 = 0;
        _rocketRangeWg3 = 0;
        _turretRangeWg3 = 0;
        _laserRangeWg3 = 0;

        foreach (var modules in moduleList)
        {
            _totalHealth += modules.ModuleData.BaseStats.health;
            _hexCount += modules.ModuleData.HexagonSize;
            _mass += modules.ModuleData.BaseStats.mass;
            _thrust += modules.ModuleData.BaseStats.thrust;
            _maxSpeed = netBridge.MaxMovementSpeed / (1 + _mass);
            _acceleration = netBridge.BaseMovementSpeed + _thrust / (1 + _mass);
            _maneuverability = netBridge.MaxAngularSpeed / (1 + _mass);
        }
        
        CalculateStatsWg1(weaponGroupListOne);
        CalculateStatsWg2(weaponGroupListTwo);
        CalculateStatsWg3(weaponGroupListThree);
        
        DisplayStats();
        // todo: make the speed calcuations more clean by getting the value directly from NetBridge
    }

    public void CalculateStatsWg1( List<NetEditorModule> weaponGroupOne)
    {
        foreach (NetEditorModule modules in weaponGroupOne)
        {
            if (modules is NetTurretEditorModule turretModule)
            {
                var turretProjectile = turretModule.ModuleScriptableObject.Projectile.baseProjectileObject;
                
                float projDmg = turretProjectile.ProjectileDamage;
                float shootingCd = turretModule.ModuleScriptableObject.Cooldown;
                float projTimer = turretProjectile.ProjectileTimer;
                float projSpeed = turretProjectile.ProjectileSpeed;
                
                _damagePerSecondWg1 += projDmg / shootingCd;
                _turretRangeWg1 = projSpeed * projTimer;
            }
            
            if (modules is NetLaserTurretEditorModule laserTurretModule)
            {
                var laserProjectile = laserTurretModule.ModuleScriptableObject.Projectile.laserProjectileObject;
                
                float projDmg = laserProjectile.ProjectileDamage;
                float shootingCd = laserTurretModule.ModuleScriptableObject.Cooldown + laserTurretModule.ModuleScriptableObject.ChargeTime;
                float maxHits = laserProjectile.MaxHits;
                float maxRangeLaser = laserProjectile.MaxLength;
                
                _damagePerSecondWg1 += (projDmg * maxHits) / shootingCd;
                _laserRangeWg1 = maxRangeLaser;
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
                _damagePerSecondWg1 += projDmg / shootingCd;
                _rocketRangeWg1 = Mathf.RoundToInt(rocketRangeUnrounded) * 10;
                
            }
            
            List<float> validValues = new List<float> { _rocketRangeWg1, _turretRangeWg1, _laserRangeWg1 }.Where(v => v != 0f).ToList();
            _maxRangeWg1 = validValues.Count > 0 ? validValues.Max() : 0f;
            _minRangeWg1 = validValues.Count > 0 ? validValues.Min() : 0f;
        }
    }
    public void CalculateStatsWg2(List<NetEditorModule> weaponGroupTwo)
    {
        foreach (NetEditorModule modules in weaponGroupTwo)
        {
            if (modules is NetTurretEditorModule turretModule)
            {
                var turretProjectile = turretModule.ModuleScriptableObject.Projectile.baseProjectileObject;
                
                float projDmg = turretProjectile.ProjectileDamage;
                float shootingCd = turretModule.ModuleScriptableObject.Cooldown;
                float projTimer = turretProjectile.ProjectileTimer;
                float projSpeed = turretProjectile.ProjectileSpeed;
                
                _damagePerSecondWg2 += projDmg / shootingCd;
                _turretRangeWg2 = projSpeed * projTimer;
            }
            
            if (modules is NetLaserTurretEditorModule laserTurretModule)
            {
                var laserProjectile = laserTurretModule.ModuleScriptableObject.Projectile.laserProjectileObject;
                
                float projDmg = laserProjectile.ProjectileDamage;
                float shootingCd = laserTurretModule.ModuleScriptableObject.Cooldown + laserTurretModule.ModuleScriptableObject.ChargeTime;
                float maxHits = laserProjectile.MaxHits;
                float maxRangeLaser = laserProjectile.MaxLength;
                
                _damagePerSecondWg2 += (projDmg * maxHits) / shootingCd;
                _laserRangeWg2 = maxRangeLaser;
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
                _damagePerSecondWg2 += projDmg / shootingCd;
                _rocketRangeWg2 = Mathf.RoundToInt(rocketRangeUnrounded) * 10;
                
            }
            
            List<float> validValues = new List<float> { _rocketRangeWg2, _turretRangeWg2, _laserRangeWg2 }.Where(v => v != 0f).ToList();
            _maxRangeWg2 = validValues.Count > 0 ? validValues.Max() : 0f;
            _minRangeWg2 = validValues.Count > 0 ? validValues.Min() : 0f;
        }
    }
    public void CalculateStatsWg3(List<NetEditorModule> weaponGroupThree)
    {
        foreach (NetEditorModule modules in weaponGroupThree)
        {
            if (modules is NetTurretEditorModule turretModule)
            {
                var turretProjectile = turretModule.ModuleScriptableObject.Projectile.baseProjectileObject;
                
                float projDmg = turretProjectile.ProjectileDamage;
                float shootingCd = turretModule.ModuleScriptableObject.Cooldown;
                float projTimer = turretProjectile.ProjectileTimer;
                float projSpeed = turretProjectile.ProjectileSpeed;
                
                _damagePerSecondWg3 += projDmg / shootingCd;
                _turretRangeWg3 = projSpeed * projTimer;
            }
            
            if (modules is NetLaserTurretEditorModule laserTurretModule)
            {
                var laserProjectile = laserTurretModule.ModuleScriptableObject.Projectile.laserProjectileObject;
                
                float projDmg = laserProjectile.ProjectileDamage;
                float shootingCd = laserTurretModule.ModuleScriptableObject.Cooldown + laserTurretModule.ModuleScriptableObject.ChargeTime;
                float maxHits = laserProjectile.MaxHits;
                float maxRangeLaser = laserProjectile.MaxLength;
                
                _damagePerSecondWg3 += (projDmg * maxHits) / shootingCd;
                _laserRangeWg3 = maxRangeLaser;
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
                _damagePerSecondWg3 += projDmg / shootingCd;
                _rocketRangeWg3 = Mathf.RoundToInt(rocketRangeUnrounded) * 10;
                
            }
            
            List<float> validValues = new List<float> { _rocketRangeWg3, _turretRangeWg3, _laserRangeWg3 }.Where(v => v != 0f).ToList();
            _maxRangeWg3 = validValues.Count > 0 ? validValues.Max() : 0f;
            _minRangeWg3 = validValues.Count > 0 ? validValues.Min() : 0f;
        }
    }

    public void DisplayStats()
    {
        totalHealth.text = $"TotalHealth: {_totalHealth:0}";
        hexCount.text = $"HexCount: {_hexCount:0}";
        maxSpeed.text = $"MaxSpeed: {_maxSpeed:0.0}";
        acceleration.text = $"Acceleration: {_acceleration:0.0}";
        maneuverability.text = $"Maneuverability: {_maneuverability:0.0}";
        //Wg1
        damagePerSecondWg1.text = $"DmgPerSecond: {_damagePerSecondWg1:0}";
        maxRangeWg1.text = $"MaxRange: {_maxRangeWg1:0}";
        minRangeWg1.text = $"MinRange: {_minRangeWg1:0}";
        //Wg2
        damagePerSecondWg2.text = $"DmgPerSecond: {_damagePerSecondWg2:0}";
        maxRangeWg2.text = $"MaxRange: {_maxRangeWg2:0}";
        minRangeWg2.text = $"MinRange: {_minRangeWg2:0}";
        //Wg3
        damagePerSecondWg3.text = $"DmgPerSecond: {_damagePerSecondWg3:0}";
        maxRangeWg3.text = $"MaxRange: {_maxRangeWg3:0}";
        minRangeWg3.text = $"MinRange: {_minRangeWg3:0}";
        
    }
}