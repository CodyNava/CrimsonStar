using UnityEngine;

namespace _01_Scripts.Ship.Modules
{
    [CreateAssetMenu(fileName = "ArmourObject", menuName = "ShipModules/ArmourObject")]
    public class ArmourModuleObject : BaseModuleObject
    {
        [SerializeField] private float additionalArmor;
    }
}