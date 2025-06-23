using UnityEngine;

public class DataProvider : SceneSingleton<DataProvider>
{
    [field: SerializeField] public NetModuleDB ModuleDB { get; private set; }
    [field: SerializeField] public GameModeConfig GameModeConfig { get; private set; }

    public static NetModuleData GetModuleDataByID(NetModuleID moduleID) => Instance.ModuleDB.ModuleData[moduleID];
    public static int GetModuleCost(NetModuleID moduleID) => Instance.ModuleDB.ModuleData[moduleID].Cost;
    public static int GetStartingCurrency(NetGameModeID gameModeID) => Instance.GameModeConfig.Descriptions[gameModeID].BaseCurrency;
    public static int GetCurrencyAddedPerRound(NetGameModeID gameModeID) => Instance.GameModeConfig.Descriptions[gameModeID].CurrencyAddedPerRound;
}
