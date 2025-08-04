using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelectionButton : MonoBehaviour
{
    [SerializeField] private Image moduleIcon;

    [SerializeField] private TMP_Text moduleName, sizeLabel, currencyLabel;
    [SerializeField] private Sprite testSprite;

    [SerializeField] Tooltip toolTip;
    public static event Action<NetModuleID> ModuleSelected;
    private NetModuleID moduleID;


    public void SpawnModule()
    {
        ModuleSelected?.Invoke(moduleID);
    }

    public void Configure(NetModuleData data)
    {
        //Base

        currencyLabel.text = $"{data.Cost}";
        moduleName.text = $"{data.DisplayName}";
        sizeLabel.text = $"{data.HexagonSize}";
        moduleIcon.sprite = data.Icon;
        moduleID = data.ModuleID;

        //ToolTips basics

        toolTip.statOne = data.HexagonSize.ToString();
        toolTip.statOneImage = testSprite;

        toolTip.statTwo = $"{data.BaseStats.health.ToString()}";
        toolTip.statTwoImage = testSprite;

        toolTip.statThree = data.CanRotate ? "Yes" : "No";
        toolTip.statThreeImage = testSprite;

        toolTip.statFour = data.BaseStats.mass.ToString();
        toolTip.statFourImage = testSprite;
        
        toolTip.statFive = data.BaseStats.thrust > 0 ? data.BaseStats.thrust.ToString() : string.Empty;
        toolTip.statFiveImage = data.BaseStats.thrust > 0 ? testSprite : null;

        toolTip.statSix = data.ShipEditorPrefab.healthOverLayData.HighHealth > 0
            ? data.EffectRange.ToString()
            : string.Empty;
        toolTip.statSixImage = data.EffectRange > 0 ? testSprite : null;

        //ToolTips Weapons

        if (data.LaserData || data.RocketData || data.TurretData)
        {
            switch (data.ModuleID)
            {
                case NetModuleID.TurretLaser:
                    //COOLDOWN
                    toolTip.statFive = data.LaserData.Cooldown > 0 ? data.LaserData.Cooldown.ToString() : string.Empty;
                    toolTip.statFiveImage = testSprite;

                    //DMG
                    toolTip.statSix = data.LaserData.Projectile.laserProjectileObject.ProjectileDamage > 0
                        ? data.LaserData.ChargeTime.ToString()
                        : string.Empty;
                    toolTip.statSixImage = testSprite;
                    break;

                case NetModuleID.TurretRocket:
                    //COOLDOWN
                    toolTip.statFive = data.RocketData.Cooldown > 0
                        ? data.RocketData.Cooldown.ToString()
                        : string.Empty;
                    toolTip.statFiveImage = testSprite;

                    //DMG
                    toolTip.statSix = data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage > 0
                        ? data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage.ToString()
                        : string.Empty;
                    toolTip.statSixImage = testSprite;
                    break;
                case NetModuleID.TurretRocketT2:
                    //COOLDOWN
                    toolTip.statFive = data.RocketData.Cooldown > 0
                        ? data.RocketData.Cooldown.ToString()
                        : string.Empty;
                    toolTip.statFiveImage = testSprite;

                    //DMG
                    toolTip.statSix = data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage > 0
                        ? data.RocketData.Projectile.rocketProjectileObject.Explosion.ExplosionDamage.ToString()
                        : string.Empty;
                    toolTip.statSixImage = testSprite;
                    break;

                case NetModuleID.Turret:
                    //COOLDOWN
                    toolTip.statFive = data.TurretData.Cooldown > 0
                        ? data.TurretData.Cooldown.ToString()
                        : string.Empty;
                    toolTip.statFiveImage = testSprite;

                    //DMG
                    toolTip.statSix = data.TurretData.Projectile.baseProjectileObject.ProjectileDamage > 0
                        ? data.TurretData.Projectile.baseProjectileObject.ProjectileDamage.ToString()
                        : string.Empty;
                    toolTip.statSixImage = testSprite;
                    break;

                case NetModuleID.TurretT2:
                    //COOLDOWN
                    toolTip.statFive = data.TurretData.Cooldown > 0
                        ? data.TurretData.Cooldown.ToString()
                        : string.Empty;
                    toolTip.statFiveImage = testSprite;

                    //DMG
                    toolTip.statSix = data.TurretData.Projectile.baseProjectileObject.ProjectileDamage > 0
                        ? data.TurretData.Projectile.baseProjectileObject.ProjectileDamage.ToString()
                        : string.Empty;
                    toolTip.statSixImage = testSprite;
                    break;
            }
        }
    }
}