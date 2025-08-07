using System;
using System.Globalization;
using _01_Scripts.Networking.ScriptableObjectScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelectionButton : MonoBehaviour
{
    [SerializeField] private Image moduleIcon, hexSizeSprite;

    [SerializeField] private TMP_Text moduleName, sizeLabel, currencyLabel;
    [SerializeField] private Sprite testSprite;
    [SerializeField] private GameObject connectorBars;

    [SerializeField] Tooltip toolTip;
    [SerializeField] SpriteDatabase database;
    private NetModuleCategory _currentData;
    public static event Action<NetModuleID> ModuleSelected;
    private NetModuleID _moduleID;

    [SerializeField] private Image buttonImage;

    [SerializeField] private Sprite weaponBut1,
        weaponBut2,
        weaponBut3,
        armorBut1,
        armorBut2,
        armorBut3,
        engineBut1,
        engineBut2,
        engineBut3,
        energyBut1,
        energyBut2,
        energyBut3;


    public void SpawnModule()
    {
        ModuleSelected?.Invoke(_moduleID);
    }

    public void EnableConnectorBars() => connectorBars.SetActive(true);


    public void DisableConnectorBars() => connectorBars.SetActive(false);

    public void GetPainterData(NetModuleCategory category) => _currentData = category;


    public void Configure(NetModuleData data)
    {
        _currentData = data.ModuleCategory;
        //Base Button
        currencyLabel.text = $"{data.Cost}";
        moduleName.text = $"{data.DisplayName}";
        sizeLabel.text = $"{data.HexagonSize}";
        moduleIcon.sprite = data.Icon;
        hexSizeSprite.sprite = data.HexSizeIcon[(int)data.HexagonSize - 1];
        _moduleID = data.ModuleID;
        toolTip.moduleDescription = data.Description;

        //ToolTips basics
        toolTip.statOne = $"{data.BaseStats.health.ToString(CultureInfo.InvariantCulture)}";
        toolTip.statOneImage = database.GetSpriteById("health");

        SetThisButtonBasedOnCategory();

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
        toolTip.statFiveImage =
            data.CanBePowered ? database.GetSpriteById("energy") : database.GetSpriteById("noenergy");
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
                var laserDamage = data.LaserData.Projectile.laserProjectileObject.ProjectileDamage;
                var laserLifeTime = data.LaserData.Projectile.laserProjectileObject.LifetimeAfterFullGrown;
                var laserTickRate = data.LaserData.Projectile.laserProjectileObject.LaserTickRate;
                var totalLaserDamage = laserDamage * laserLifeTime / laserTickRate;
                toolTip.statTwo = totalLaserDamage.ToString(CultureInfo.InvariantCulture);

                //COOLDOWN
                toolTip.statThree = data.LaserData.Cooldown.ToString(CultureInfo.InvariantCulture);

                //PROJECTILE TYPE
                toolTip.statFour =
                    data.LaserData.Projectile.laserProjectileObject.MaxTargetsPerHit.ToString(CultureInfo
                        .InvariantCulture);
                toolTip.statFourImage = database.GetSpriteById("piercing");
                break;
            case NetModuleID.DeepPenLaser:
                //DMG
                var deepPenDamage = data.LaserData.Projectile.laserProjectileObject.ProjectileDamage;
                var deepPenLifeTime = data.LaserData.Projectile.laserProjectileObject.LifetimeAfterFullGrown;
                var deepPenTickRate = data.LaserData.Projectile.laserProjectileObject.LaserTickRate;
                var totalDeepPenLaserDamage = deepPenDamage * deepPenLifeTime / deepPenTickRate;
                toolTip.statTwo = totalDeepPenLaserDamage.ToString(CultureInfo.InvariantCulture);

                //COOLDOWN
                toolTip.statThree = data.LaserData.Cooldown.ToString(CultureInfo.InvariantCulture);

                //PROJECTILE TYPE
                toolTip.statFour =
                    data.LaserData.Projectile.laserProjectileObject.MaxTargetsPerHit.ToString(CultureInfo
                        .InvariantCulture);
                toolTip.statFourImage = database.GetSpriteById("piercing");
                break;
            case NetModuleID.TurretRocket:
                //DMG
                toolTip.statTwo =
                    data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage.ToString(CultureInfo
                        .InvariantCulture);

                //COOLDOWN
                toolTip.statThree = data.RocketData.Cooldown.ToString(CultureInfo.InvariantCulture);

                //PROJECTILE TYPE
                var torpedoExplosionSize =
                    data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionMaxSize / 2;
                toolTip.statFour = torpedoExplosionSize.ToString(CultureInfo.InvariantCulture);
                toolTip.statFourImage = database.GetSpriteById("aoe");
                break;
            case NetModuleID.TurretRocketT2:
                //DMG
                toolTip.statTwo =
                    data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage.ToString(CultureInfo
                        .InvariantCulture);

                //COOLDOWN
                toolTip.statThree = data.RocketData.Cooldown.ToString(CultureInfo.InvariantCulture);

                //PROJECTILE TYPE
                var batteryExplosionSize =
                    data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionMaxSize / 2;
                toolTip.statFour = batteryExplosionSize.ToString(CultureInfo.InvariantCulture);
                toolTip.statFourImage = database.GetSpriteById("aoe");
                break;
            case NetModuleID.Turret:
                //DMG
                toolTip.statTwo =
                    data.TurretData.Projectile.baseProjectileObject.ProjectileDamage.ToString(CultureInfo
                        .InvariantCulture);

                //COOLDOWN
                toolTip.statThree = data.TurretData.Cooldown.ToString(CultureInfo.InvariantCulture);

                //PROJECTILE TYPE
                toolTip.statFour = ".";
                toolTip.statFourImage = database.GetSpriteById("kinetic");
                break;
            case NetModuleID.ShredderGun:
                //DMG
                toolTip.statTwo =
                    data.TurretData.Projectile.baseProjectileObject.ProjectileDamage.ToString(CultureInfo
                        .InvariantCulture);

                //COOLDOWN
                toolTip.statThree = data.TurretData.Cooldown.ToString(CultureInfo.InvariantCulture);

                //PROJECTILE TYPE
                toolTip.statFour = ".";
                toolTip.statFourImage = database.GetSpriteById("kinetic");
                break;
        }
    }


    private void SetThisButtonBasedOnCategory()
    {
        switch (_currentData)
        {
            case NetModuleCategory.Weapons:
                buttonImage.sprite = weaponBut1;
                break;
            case NetModuleCategory.Armor:
                buttonImage.sprite = armorBut1;
                break;
            case NetModuleCategory.Engines:
                buttonImage.sprite = engineBut1;
                break;
            case NetModuleCategory.Energy:
                buttonImage.sprite = energyBut1;
                break;
        }
    }

    public void ChangeButtonStateBasedOnCategory()
    {
        var data = _currentData;
        switch (data)
        {
            case NetModuleCategory.Weapons:
                buttonImage.sprite = buttonImage.sprite == weaponBut2 
                    ? weaponBut1
                    : weaponBut2;
                return;
            case NetModuleCategory.Armor:
                buttonImage.sprite = buttonImage.sprite == armorBut2
                    ? armorBut1
                    : armorBut2;
                return;
            case NetModuleCategory.Engines:
                buttonImage.sprite = buttonImage.sprite == engineBut2
                    ? engineBut1
                    : engineBut2;
                return;
            case NetModuleCategory.Energy:
                buttonImage.sprite = buttonImage.sprite == energyBut2
                    ? energyBut1
                    : energyBut2;
                return;
        }
    }
    
    public void ChangeButtonClickedBasedOnCategory()
    {
        var data = _currentData;
        switch (data)
        {
            case NetModuleCategory.Weapons:
                buttonImage.sprite = buttonImage.sprite == weaponBut3
                    ? weaponBut2
                    : weaponBut3;
                return;
            case NetModuleCategory.Armor:
                buttonImage.sprite = buttonImage.sprite == armorBut3
                    ? armorBut2
                    : armorBut3;
                return;
            case NetModuleCategory.Engines:
                buttonImage.sprite = buttonImage.sprite == engineBut3
                    ? engineBut2
                    : engineBut3;
                return;
            case NetModuleCategory.Energy:
                buttonImage.sprite = buttonImage.sprite == energyBut3
                    ? energyBut2
                    : energyBut3;
                return;
        }
    }
}