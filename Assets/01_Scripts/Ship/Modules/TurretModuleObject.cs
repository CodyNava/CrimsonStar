using UnityEngine;
using Object = System.Object;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "TurretModuleObject", menuName = "ShipModules/TurretObject")]
    public class TurretModuleObject : BaseModuleObject
    {
        public float _reloadTime;
        public Object _projectile;
    }
}