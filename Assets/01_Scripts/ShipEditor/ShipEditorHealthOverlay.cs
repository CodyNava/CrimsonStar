using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using _01_Scripts.Ship;
using _01_Scripts.Ship.ModuleControllers;
using FishNet.Component.Prediction;
using FishNet.Connection;
using FishNet.Transporting;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;

public static class ShipEditorHealthOverlay
{
    private static readonly Dictionary<HexCoordinate, float> _healthMap = new ();
    public static IReadOnlyDictionary<HexCoordinate, float>  HealthMap => _healthMap;


    public static void WriteHealthMap(HexCoordinate coord, float health)
    {
        _healthMap[coord] = health;
    }
    public static void RemoveHealthMap(HexCoordinate coord)
    {
        _healthMap.Remove(coord);
    }
    public static void ClearHealthMap()
    {
        _healthMap.Clear();
    }


}
