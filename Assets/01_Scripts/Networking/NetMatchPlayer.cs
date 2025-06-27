using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;

public class NetMatchPlayer : NetworkBehaviour
{
    public readonly SyncVar<NetTeamID> Team = new();
    public readonly SyncVar<ulong> PlayerID = new();
    public readonly SyncVar<string> DisplayName = new();
    public readonly SyncVar<int> ResourceCount = new();
    public readonly SyncVar<float> DamageReceivedRound = new();
    public readonly SyncVar<float> DamageReceivedMatch = new();
    public readonly SyncVar<float> DamageDealtRound = new();
    public readonly SyncVar<float> DamageDealtMatch = new();
    public readonly SyncVar<int> KillsRound = new();
    public readonly SyncVar<int> KillsMatch = new();
    public readonly SyncVar<int> MatchScore = new();
    public readonly SyncVar<bool> Survived = new();
    
    public NetModuleStorage ModuleStorage { get; private set; }

    [Server]
    public void S_Init(NetLobbyData lobbyData, NetGameModeID gameModeID)
    {
        Team.Value = lobbyData.playerTeamID;
        PlayerID.Value = lobbyData.playerID;
        DisplayName.Value = lobbyData.playerDisplayName;
        ModuleStorage ??= GetComponent<NetModuleStorage>();
        ModuleStorage.Init();
        ResourceCount.Value = DataProvider.GetStartingCurrency(gameModeID);
        C_Init();
    }

    [Server]
    public void S_ResetRoundStats()
    {
        DamageReceivedRound.Value = 0;
        DamageDealtRound.Value = 0;
        Survived.Value = true;
    }

    [ObserversRpc][Client]
    private void C_Init()
    {
        ModuleStorage ??= GetComponent<NetModuleStorage>();
        ModuleStorage.Init();
    }

    [Client]
    public bool C_CanAffordModule(NetModuleID moduleID)
    {
        if (!IsOwner) return false;
        return ResourceCount.Value >= DataProvider.GetModuleCost(moduleID);
    }

    [Client]
    public void C_PayForModule(NetModuleID moduleID)
    {
        if (IsOwner)
        {
            S_PayForModule(moduleID);
        }
    }

    [Client]
    public void C_RefundModule(NetModuleID moduleID)
    {
        if (IsOwner)
        {
            S_RefundModule(moduleID);
        }
    }
    
    [ServerRpc][Server]
    private void S_PayForModule(NetModuleID moduleID)
    {
        ResourceCount.Value -= DataProvider.GetModuleCost(moduleID);
    }
    
    [ServerRpc][Server]
    private void S_RefundModule(NetModuleID moduleID)
    {
        ResourceCount.Value += DataProvider.GetModuleCost(moduleID);
    }

    [Client]
    public bool C_SignalReady()
    {
        if (IsOwner)
        {
            InstanceFinder.GetInstance<NetShipEditorConductor>().S_SignalReady();
            return true;
        }

        return false;
    }
}
