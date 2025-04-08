using UnityEngine;

namespace _01_Scripts.Ship.Modules
{
    public abstract class BaseModuleObject : ScriptableObject
    {
        [SerializeField] private int _cost;
        [SerializeField] private float _health;
        [SerializeField] private float _weight;
    }
}