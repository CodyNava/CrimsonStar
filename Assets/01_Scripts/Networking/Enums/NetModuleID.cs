using System.Collections.Generic;

public enum NetModuleID
{
    Unknown = 0,
    Bridge = 1,
    Armor = 2,
    Thruster = 3,
    Turret = 4,
    TurretLaser = 5,
    Reactor = 6,
    TurretRocket = 7,
    TurretT2 = 8
}

public static class ModuleIDExtensions
{
    public static NetModuleData GetModuleData(this NetModuleID moduleID)
    {
        return DataProvider.Instance.ModuleDB.ModuleData.GetValueOrDefault(moduleID);
    }
}
