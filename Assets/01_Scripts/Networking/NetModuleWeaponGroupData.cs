using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class NetModuleWeaponGroupData
{
    private static readonly Dictionary<HexCoordinate, int> _weaponGroupMap = new();

    public static IReadOnlyDictionary<HexCoordinate, int> WeaponGroupMap => _weaponGroupMap;

    public static void WriteWeaponGroup(HexCoordinate coord, int weaponGroup)
    {
        _weaponGroupMap[coord] = weaponGroup; //auf key gruppe setzen
    }
    
    public static void RemoveWeaponGroup(HexCoordinate coord)
    {
        _weaponGroupMap.Remove(coord); //diesen key(coord) töten
    }

    public static void ClearAllWeaponGroupKeys()
    {
        _weaponGroupMap.Clear(); //alle keys töten
        
    }
}
