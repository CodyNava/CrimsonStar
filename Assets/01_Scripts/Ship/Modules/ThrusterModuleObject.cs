using UnityEngine;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "ThrusterObject", menuName = "ShipModules/ThrusterObject")]
    public class ThrusterModuleObject : BaseModuleObject
    {
        public float _thrust;
    }
}