using FishNet;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
public class ExplosiveBarrel : NetworkBehaviour
{
    [SerializeField] private ExplosionObject explosionObject;
    private readonly SyncVar<NetTeamID> _netTeamID = new SyncVar<NetTeamID>();
    private ulong _attackerID = 0;

    
    public void Initialize(float passedTime, NetTeamID netTeamID, ulong attackerID = 0)
    {
        _netTeamID.Value = netTeamID;
        _attackerID = attackerID;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            S_SpawnExplosion(transform.position, _attackerID);
        }
    }
    
    [Server]
    private void S_SpawnExplosion(Vector3 pos, ulong senderID)
    {
        C_SpawnExplosion(pos, senderID);
        C_ObserversSpawnExplosion(pos, senderID, Channel.Reliable);
    }

    [ObserversRpc]
    private void C_ObserversSpawnExplosion(Vector3 pos, ulong senderID, Channel channel = Channel.Reliable)
    {
        C_SpawnExplosion(pos, senderID);
    }

    private void C_SpawnExplosion(Vector3 position, ulong senderID)
    {
        NetPredictedExplosion pe = Instantiate(explosionObject.ExplosionPrefab, position, Quaternion.identity);
        pe.Initialize(_netTeamID.Value, explosionObject, senderID);
        Destroy(gameObject);
    }
}
