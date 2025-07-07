using UnityEngine;
using Random = System.Random;
using FishNet;

public class Nebula : MonoBehaviour
{
    [SerializeField] private NebulaObject _nebulaObject;
    [SerializeField] private GameObject hitFeedbackVFX;
    
    private ulong _attackerID;
    private NetTeamID _netTeamID;
    private float dt;
    
    
    public void Initialize(float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _netTeamID = netTeamID;
        _attackerID = attackerID;
    }
    
    public void Start()
    {
        Random rnd = new Random();
        int size = rnd.Next(_nebulaObject.NebulaMinSize, _nebulaObject.NebulaMaxSize+1);
        gameObject.transform.localScale = Vector3.one * size;
    }

    public void Update()
    {
        dt = Time.deltaTime;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.NetTeamID == _netTeamID) return;

        if (dt > _nebulaObject.NebulaDamageInterval)
        {
            if (InstanceFinder.IsClientStarted)
            {
                // Visual and Audio
            }
            if (InstanceFinder.IsServerStarted)
            {
                module.S_InflictDamage(_nebulaObject.NebulaDamage, _attackerID);
            }
            dt = 0;
        }
    }
}
