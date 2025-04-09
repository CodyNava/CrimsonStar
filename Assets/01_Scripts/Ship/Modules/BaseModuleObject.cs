using UnityEngine;

namespace _01_Scripts.Ship.Modules
{
    public abstract class BaseModuleObject : ScriptableObject
    {
        public int _cost;
        public float _health;
        public float _weight;
    }
}