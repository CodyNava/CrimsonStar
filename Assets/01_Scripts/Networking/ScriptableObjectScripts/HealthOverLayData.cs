using UnityEngine;

[CreateAssetMenu(fileName = "Overlay", menuName = "Overlay/Health")]
public class HealthOverLayData : ScriptableObject
{
    [Tooltip("Here you can Setup the value and color of Modules in the HealthView (TotalMode)\n" +
             " Example: LowHealth = 80, LowHealthColor = Red, MidHealth = 150, MidHealthColor = Yellow\n " +
             " if an module has 80 or less health it will be shown red\n " +
             "if  an module has more than 80 but less than 150 it will be shown yellow")]
    [Header("TotalHealthOverlay")]
    [field: SerializeField]
    public float LowHealth { get; private set; }

    [field: SerializeField] public Color LowHealthColor { get; private set; }

    [field: SerializeField] public float MidHealth { get; private set; }
    [field: SerializeField] public Color MidHealthColor { get; private set; }

    [field: SerializeField] public float HighHealth { get; private set; }
    [field: SerializeField] public Color HighHealthColor { get; private set; }
    [field: SerializeField] public float SuperHighHealth { get; private set; }
    [field: SerializeField] public Color SuperHighHealthColor { get; private set; }

    [Tooltip("The Lowest Value in Percentage Mode" +
             "\n These Two are are a Range imagen Lowest = 0, Highest = 1\n" +
             "Example: LowestPercentageColor = Red, HighestPercentageColor = Green\n" +
             "You have placed a Turret(80hp), Armour(120hp), RocketT2(150hp)\n" +
             "the Turret would be Red, the rocketT2 Green, and Armour would be the colour in Between in our Example Light Yellow'ish")]
    [Header("PercentageHealthOverlay")]
    [field: SerializeField]
    public Color LowestPercentageColor { get; private set; }

    [field: SerializeField] public Color HighestPercentageColor { get; private set; }
}