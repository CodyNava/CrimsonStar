using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipEditorStats : MonoBehaviour
{
    [Header("totalStats")]
    [SerializeField] private TMP_Text totalHealth;
    [SerializeField] private TMP_Text hexCount;
    private float _totalHealth;
    private int _hexCount;

    [Header("WeaponGroup1")]
    [SerializeField] private TMP_Text damagePerSecond;
    private float _damagePerSecond;

    [Header("Speed")]
    [SerializeField] private TMP_Text maxSpeed;
    private float _maxSpeed;

    public void GetTotalStats(IReadOnlyList<NetEditorModule> moduleList)
    {
        _totalHealth = 0;
        _hexCount = 0;
        _damagePerSecond = 0;
        _maxSpeed = 0;

        foreach (NetEditorModule modules in moduleList)
        {
            _totalHealth += modules.ModuleData.BaseStats.health;
        }
        DisplayStats();
        // todo: write classes that inherite from netEditorModule to access -
        // specialized scriplableobjects such as projectiles.
    }
    public void DisplayStats()
    {
        totalHealth.text = $"Total Health: {_totalHealth}";
        hexCount.text = $"Hex Count: {_hexCount}";
        damagePerSecond.text = $"Damage Per Second: {_damagePerSecond}";
        maxSpeed.text = $"Max Speed: {_maxSpeed}";
    }
}
