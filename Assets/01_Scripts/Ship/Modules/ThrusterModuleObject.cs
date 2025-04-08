using UnityEngine;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "ThrusterModuleObject", menuName = "ShipModules/ThrusterObject", order = 1)]
    public class ThrusterModuleObject : BaseModuleObject
    {
        [SerializeField] private float thrust;
    }
}