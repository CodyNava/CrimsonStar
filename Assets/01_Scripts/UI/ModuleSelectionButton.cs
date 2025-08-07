using System;
using System.Globalization;
using _01_Scripts.Networking.ScriptableObjectScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelectionButton : MonoBehaviour
{
    [SerializeField] private Image moduleIcon,  hexSizeSprite;

    [SerializeField] private TMP_Text moduleName, sizeLabel, currencyLabel;
    [SerializeField] private Sprite testSprite;
    [SerializeField] private GameObject connectorBars;

    [SerializeField] Tooltip toolTip;
    [SerializeField] SpriteDatabase database;
    public static event Action<NetModuleID> ModuleSelected;
    private NetModuleID _moduleID;


    public void SpawnModule()
    {
        ModuleSelected?.Invoke(_moduleID);
        
    }

    public void EnableConnectorBars() => connectorBars.SetActive(true);
    
    
    public void DisableConnectorBars() => connectorBars.SetActive(false);
    

    public void Configure(NetModuleData data)
    {
        //Base Button
        currencyLabel.text = $"{data.Cost}";
        moduleName.text = $"{data.DisplayName}";
        sizeLabel.text = $"{data.HexagonSize}";
        moduleIcon.sprite = data.Icon;
        hexSizeSprite.sprite = data.HexSizeIcon[(int)data.HexagonSize -1 ];
        _moduleID = data.ModuleID;
        toolTip.moduleDescription = data.Description;

        //ToolTips basics
        toolTip.statOne = $"{data.BaseStats.health.ToString(CultureInfo.InvariantCulture)}";
        toolTip.statOneImage = database.GetSpriteById("health");
        
        
        switch (data.ModuleCategory)
        {
            case NetModuleCategory.Weapons:
                WeaponToolTip(data);
                break;
            case NetModuleCategory.Engines:
                ThrusterToolTip(data);
                break;
            case NetModuleCategory.Energy:
                ReactorToolTip(data);
                break;
            case NetModuleCategory.Armor:
                ArmorToolTip(data);
                break;
        }
    }


    private void ThrusterToolTip(NetModuleData data)
    {
        //Thrust
        toolTip.statTwo = data.BaseStats.thrust.ToString(CultureInfo.InvariantCulture);
        toolTip.statTwoImage = database.GetSpriteById("thrust");

        //Rotation
        toolTip.statThree = data.BaseStats.angularThrust.ToString(CultureInfo.InvariantCulture);
        toolTip.statThreeImage = database.GetSpriteById("rotationspeed");
        
        //Mass
        var betterMassNumbers = data.BaseStats.mass;
        toolTip.statFour = betterMassNumbers.ToString(CultureInfo.InvariantCulture) + "%";
        toolTip.statFourImage = database.GetSpriteById("maxspeed");
    }

    private void ArmorToolTip(NetModuleData data)
    {
        //Mass
        var betterMassNumbers = data.BaseStats.mass;
        toolTip.statTwo = betterMassNumbers.ToString(CultureInfo.InvariantCulture) + "%";
        toolTip.statTwoImage = database.GetSpriteById("maxspeed");
    }

    private void ReactorToolTip(NetModuleData data)
    {
        //Mass
        var betterMassNumbers = data.BaseStats.mass;
        toolTip.statTwo = betterMassNumbers.ToString(CultureInfo.InvariantCulture) + "%";
        toolTip.statTwoImage = database.GetSpriteById("maxspeed");
    }


    private void WeaponToolTip(NetModuleData data)
    {
        //General 
        //Damage
        toolTip.statTwoImage = database.GetSpriteById("damage");
        //Atkspeed
        toolTip.statThreeImage = database.GetSpriteById("atkspeed");
        //Energy
        toolTip.statFiveImage = data.CanBePowered ? database.GetSpriteById("energy") : database.GetSpriteById("noenergy");
        toolTip.statFive = ".";
        //Mass
        var betterMassNumbers = data.BaseStats.mass;
        toolTip.statSix = betterMassNumbers.ToString(CultureInfo.InvariantCulture) + "%";
        toolTip.statSixImage = database.GetSpriteById("maxspeed");
        
        //Unique
        switch (data.ModuleID)
            {
                case NetModuleID.TurretLaser:
                    //DMG
                    toolTip.statTwo = data.LaserData.Projectile.laserProjectileObject.ProjectileDamage.ToString(CultureInfo.InvariantCulture);
                    
                    //COOLDOWN
                    toolTip.statThree = data.LaserData.Cooldown.ToString(CultureInfo.InvariantCulture);
                    
                    //PROJECTILE TYPE
                    toolTip.statFour = data.LaserData.Projectile.laserProjectileObject.MaxTargetsPerHit.ToString(CultureInfo.InvariantCulture);
                    toolTip.statFourImage = database.GetSpriteById("piercing");
                    break;
                case NetModuleID.TurretRocket:
                    //DMG
                    toolTip.statTwo = data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage.ToString(CultureInfo.InvariantCulture);
                    
                    //COOLDOWN
                    toolTip.statThree = data.RocketData.Cooldown.ToString(CultureInfo.InvariantCulture);
                    
                    //PROJECTILE TYPE
                    toolTip.statFour = data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionMaxSize.ToString(CultureInfo.InvariantCulture);
                    toolTip.statFourImage = database.GetSpriteById("aoe");
                    break;
                case NetModuleID.TurretRocketT2:
                    //DMG
                    toolTip.statTwo = data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage.ToString(CultureInfo.InvariantCulture);
                    
                    //COOLDOWN
                    toolTip.statThree = data.RocketData.Cooldown.ToString(CultureInfo.InvariantCulture);
                    
                    //PROJECTILE TYPE
                    toolTip.statFour = data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionMaxSize.ToString(CultureInfo.InvariantCulture);
                    toolTip.statFourImage = database.GetSpriteById("aoe");
                    break;
                case NetModuleID.Turret:
                    //DMG
                    toolTip.statTwo = data.TurretData.Projectile.baseProjectileObject.ProjectileDamage.ToString(CultureInfo.InvariantCulture);
                    
                    //COOLDOWN
                    toolTip.statThree = data.TurretData.Cooldown.ToString(CultureInfo.InvariantCulture);
                    
                    //PROJECTILE TYPE
                    toolTip.statFour = ".";
                    toolTip.statFourImage = database.GetSpriteById("kinetic");
                    break;
                case NetModuleID.ShredderGun:
                    //DMG
                    toolTip.statTwo = data.TurretData.Projectile.baseProjectileObject.ProjectileDamage.ToString(CultureInfo.InvariantCulture);
                    
                    //COOLDOWN
                    toolTip.statThree = data.TurretData.Cooldown.ToString(CultureInfo.InvariantCulture);
                    
                    //PROJECTILE TYPE
                    toolTip.statFour = ".";
                    toolTip.statFourImage = database.GetSpriteById("kinetic");
                    break;
            }
    }
}