using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public class GameModeDescription
{
    public int BaseCurrency;
    public int CurrencyAddedPerRound;
    public GameModeDescription() {}
    public GameModeDescription(int baseCurrency, int currencyAddedPerRound)
    {
        BaseCurrency = baseCurrency;
        CurrencyAddedPerRound = currencyAddedPerRound;
    }
}

[CreateAssetMenu(fileName = "Game Mode Config", menuName = "GameModeConfig", order = 0)]
public class GameModeConfig : ScriptableObject
{
    [SerializedDictionary("Game Mode", "Mode Description")] [field: SerializeField]
    public SerializedDictionary<NetGameModeID, GameModeDescription> Descriptions { get; private set; }

    public int GetBaseCurrency(NetGameModeID gameModeID) => gameModeID == NetGameModeID.Custom ? DataProvider.Instance.customGameMode.BaseCurrency : Descriptions[gameModeID].BaseCurrency;
    public int GetCurrencyAddedPerRound(NetGameModeID gameModeID) =>  gameModeID == NetGameModeID.Custom ? DataProvider.Instance.customGameMode.CurrencyAddedPerRound : Descriptions[gameModeID].CurrencyAddedPerRound;
}
