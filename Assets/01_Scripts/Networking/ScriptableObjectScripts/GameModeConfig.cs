using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public class GameModeDescription
{
    public int BaseCurrency;
    public int CurrencyAddedPerRound;
}

[CreateAssetMenu(fileName = "Game Mode Config", menuName = "GameModeConfig", order = 0)]
public class GameModeConfig : ScriptableObject
{
    [SerializedDictionary("Game Mode", "Mode Description")] [field: SerializeField]
    public SerializedDictionary<NetGameModeID, GameModeDescription> Descriptions { get; private set; }
}
