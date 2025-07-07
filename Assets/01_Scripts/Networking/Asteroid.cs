using UnityEngine;
using FishNet;
using Random = System.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private AsteroidObject _asteroidObject;
    [SerializeField] private GameObject hitFeedbackVFX;
    
    private ulong _attackerID;
    private NetTeamID _netTeamID;
    
    
    public void Initialize(float passedTime, NetTeamID netTeamID, ulong attackerID)
    {
        _netTeamID = netTeamID;
        _attackerID = attackerID;
    }

    public void Start()
    {
        Random rnd = new Random();
        int size = rnd.Next(_asteroidObject.AsteroidMinSize, _asteroidObject.AsteroidMaxSize+1);
        gameObject.transform.localScale = Vector3.one * size;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module) || module.NetTeamID == _netTeamID) return;
        
        
        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            module.S_InflictDamage(_asteroidObject.AsteroidDamage, _attackerID);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
