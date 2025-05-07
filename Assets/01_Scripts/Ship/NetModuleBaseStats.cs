[System.Serializable]
public struct NetModuleBaseStats
{
    public float health;
    public float mass;
    public float thrust;
    public float energyCapacity;
    public float energyProduction;

    public NetModuleBaseStats Combine(NetModuleBaseStats other)
    {
        return new NetModuleBaseStats
        {
            health = health + other.health,
            mass = mass + other.mass,
            thrust = thrust + other.thrust,
            energyCapacity = energyCapacity + other.energyCapacity,
            energyProduction = energyProduction + other.energyProduction,
        };
    }

    public NetModuleBaseStats Subtract(NetModuleBaseStats other)
    {
        return new NetModuleBaseStats
        {
            health = health - other.health,
            mass = mass - other.mass,
            thrust = thrust - other.thrust,
            energyCapacity = energyCapacity - other.energyCapacity,
            energyProduction = energyProduction - other.energyProduction,
        };
    }
}