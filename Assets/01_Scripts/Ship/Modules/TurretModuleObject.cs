using UnityEngine;
using Object = System.Object;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "TurretModuleObject", menuName = "ShipModules/TurretObject")]
    public class TurretModuleObject : BaseModuleObject
    {
        [SerializeField] private float _reloadTime;
        [SerializeField] private Object _projectile;
    }
}