using UnityEngine;
using Object = System.Object;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "TurretModuleObject", menuName = "ShipModules/TurretObject", order = 1)]
    public class TurretModuleObject : BaseModuleObject
    {
        [SerializeField] private float reloadTime;
        [SerializeField] private Object projectile;
    }
}