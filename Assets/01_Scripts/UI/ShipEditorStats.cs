using System.Collections.Generic;
using System.Linq;
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
    private float _maneuverablility;

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
        _maneuverablility = 0;
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
            _maneuverablility = netBridge.MaxAngularSpeed / (1 + _mass);

            if (modules is NetTurretEditorModule turretModule)
            {
                float projDmg = turretModule.ModuleScriptableObject.Projectile.baseProjectileObject.ProjectileDamage;
                float shootingCd = turretModule.ModuleScriptableObject.Cooldown;
                _damagePerSecond += projDmg / shootingCd;
                float projTimer = turretModule.ModuleScriptableObject.Projectile.baseProjectileObject.ProjectileTimer;
                float projSpeed = turretModule.ModuleScriptableObject.Projectile.baseProjectileObject.ProjectileSpeed;
                _turretRange = projSpeed * projTimer;
            }
            

            if (modules is NetLaserTurretEditorModule laserTurretModule)
            {
                float projDmg = laserTurretModule.ModuleScriptableObject.Projectile.laserProjectileObject
                    .ProjectileDamage;
                float shootingCd = laserTurretModule.ModuleScriptableObject.Cooldown + laserTurretModule.ModuleScriptableObject.ChargeTime;
                float maxHits = laserTurretModule.ModuleScriptableObject.Projectile.laserProjectileObject.MaxHits;
                _damagePerSecond += (projDmg * maxHits) / shootingCd;
                float maxRangeLaser =
                    laserTurretModule.ModuleScriptableObject.Projectile.laserProjectileObject.MaxLength;
                _laserRange = maxRangeLaser;
            }
            

            if (modules is NetRocketEditorModule rocketEditorModule)
            {
                float projDmg = rocketEditorModule.ModuleScriptableObject.Projectile.rocketProjectileObject
                    .ProjectileDamage;
                //float explDmg = rocketEditorModule.ModuleScriptableObject.Projectile.rocketProjectileObject.   //todo explosion damage beachten
                float shootingCd = rocketEditorModule.ModuleScriptableObject.Cooldown;
                _damagePerSecond += projDmg / shootingCd;
                float rocketTimer = rocketEditorModule.ModuleScriptableObject.Projectile.rocketProjectileObject
                    .ProjectileTimer;
                float rocketSpeed = rocketEditorModule.ModuleScriptableObject.Projectile.rocketProjectileObject
                    .ProjectileAcceleration;
                _rocketRange = rocketSpeed * rocketTimer;
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
        totalHealth.text = $"TotalHealth: {_totalHealth:0.0}";
        hexCount.text = $"HexCount: {_hexCount}";
        damagePerSecond.text = $"DmgPerSecond: {_damagePerSecond:0.0}";
        maxSpeed.text = $"MaxSpeed: {_maxSpeed:0.0}";
        acceleration.text = $"Acceleration: {_acceleration:0.0}";
        maneuverability.text = $"Maneuverablility: {_maneuverablility:0.0}";
        maxRange.text = $"MaxRange: {_maxRange:0.0}";
        minRange.text = $"MinRange: {_minRange:0.0}";
    }
}