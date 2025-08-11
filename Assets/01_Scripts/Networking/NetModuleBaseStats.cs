[System.Serializable]
public struct NetModuleBaseStats
{
    public float health;
    public float mass;
    public float thrust;
    public float angularThrust;
    public float weight;

    public NetModuleBaseStats Combine(NetModuleBaseStats other)
    {
        return new NetModuleBaseStats
        {
            health = health + other.health,
            mass = mass + other.mass,
            thrust = thrust + other.thrust,
            weight = weight + other.weight
        };
    }

    public NetModuleBaseStats Subtract(NetModuleBaseStats other)
    {
        return new NetModuleBaseStats
        {
            health = health - other.health,
            mass = mass - other.mass,
            thrust = thrust - other.thrust,
            weight = weight - other.weight
        };
    }
}