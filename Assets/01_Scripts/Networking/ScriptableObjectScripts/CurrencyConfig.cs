using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public class CurrencyDisplayInfo
{
    public string abbreviation;
}

[CreateAssetMenu(fileName = "Currency Config", menuName = "Configs/Currency Config", order = 0)]
public class CurrencyConfig : ScriptableObject
{
    [SerializedDictionary("Currency", "Currency Display Info")]
    [field: SerializeField] public SerializedDictionary<NetCurrencyType, CurrencyDisplayInfo> CurrencyDisplayInfos { get; set; }
}
