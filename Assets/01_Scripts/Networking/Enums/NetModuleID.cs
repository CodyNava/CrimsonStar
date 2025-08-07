using System.Collections.Generic;

public enum NetModuleID
{
    Unknown = 0,
    Bridge = 1,
    Armor = 2,
    Thruster = 3,
    Turret = 4,
    ShredderGun = 5,
    Reactor = 6,
    RailGun = 7,
    TurretLaser = 8,
    DeepPenLaser = 9,
    TurretRocket = 10,
    HullPlating = 11,
    HullFinish = 12,
    NavigationThruster = 13,
    TurretRocketT2 = 14
}

public static class ModuleIDExtensions
{
    public static NetModuleData GetModuleData(this NetModuleID moduleID)
    {
        return DataProvider.Instance.ModuleDB.ModuleData.GetValueOrDefault(moduleID);
    }
}
