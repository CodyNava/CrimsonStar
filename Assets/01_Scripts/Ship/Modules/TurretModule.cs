using UnityEngine;
using Object = System.Object;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "TurretModule", menuName = "ShipModules/TurretModule", order = 1)]
    public class TurretModule : BaseShipModule
    {
        public float reloadTime;
        public Object projectile;
    }
}