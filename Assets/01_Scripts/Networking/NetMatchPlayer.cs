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

    public void S_ResetRoundStats()
    {
        DamageReceivedRound.Value = 0;
        DamageDealtRound.Value = 0;
        Survived.Value = true;
    }

    [ObserversRpc]
    private void C_Init()
    {
        ModuleStorage ??= GetComponent<NetModuleStorage>();
        ModuleStorage.Init();
    }

    public bool C_CanAffordModule(NetModuleID moduleID)
    {
        if (!IsOwner) return false;
        return ResourceCount.Value >= DataProvider.GetModuleCost(moduleID);
    }

    public void C_PayForModule(NetModuleID moduleID)
    {
        if (IsOwner)
        {
            S_PayForModule(moduleID);
        }
    }

    public void C_RefundModule(NetModuleID moduleID)
    {
        if (IsOwner)
        {
            S_RefundModule(moduleID);
        }
    }
    
    [ServerRpc]
    private void S_PayForModule(NetModuleID moduleID)
    {
        ResourceCount.Value -= DataProvider.GetModuleCost(moduleID);
    }
    
    [ServerRpc]
    private void S_RefundModule(NetModuleID moduleID)
    {
        ResourceCount.Value += DataProvider.GetModuleCost(moduleID);
    }

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
