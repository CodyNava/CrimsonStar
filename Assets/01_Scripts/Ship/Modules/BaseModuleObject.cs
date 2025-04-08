using UnityEngine;

namespace _01_Scripts.Ship.Modules
{
    public abstract class BaseModuleObject : ScriptableObject
    {
        [SerializeField] private float health;
        [SerializeField] private float weight;
    }
}