using _01_Scripts.Ship.Modules;

namespace _01_Scripts.Ship.ModuleControllers
{
    public class BridgeController : BaseModuleController
    {
        public BridgeModuleObject BridgeObject => (BridgeModuleObject)ModuleObject;
        public float MaxHp { get; private set; }

        public void AddModule(BaseModuleObject modulAdded)
        {
            MaxHp += modulAdded._health;
        }
        
        public void RemoveModule(BaseModuleObject modulAdded)
        {
            MaxHp -= modulAdded._health;
        }
    }
}