using UnityEngine;
using FishNet;
using Random = System.Random;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Asteroid : MonoBehaviour
{
    private AsteroidObject _asteroidObject;
    [SerializeField] private GameObject hitFeedbackVFX;


    private ulong _attackerID = 0;
    private NetTeamID _netTeamID;
    private Vector3 _direction;
    private CircleCollider2D circleCollider;
    private Vector3 _moveDirection;
    private float _speed;
    private float _boundX;
    private float _boundY;
    private Vector3 _center;

    private NetBridge _bridgeOrigin;
    private NetLobbyConductor _lobbyConductor;
    
    private HashSet<NetGameplayModule> hitModules = new HashSet<NetGameplayModule>();
    
    public void Initialize(NetTeamID netTeamID, AsteroidObject asteroidObject, Vector3 direction, float moveSpeed, float boxWidth, float boxHeight, Vector3 centerPosition, ulong attackerID = 0, NetBridge bridgeOrigin = null)
    {
        _netTeamID = netTeamID;
        _attackerID = attackerID;
        _asteroidObject = asteroidObject;
        _bridgeOrigin = bridgeOrigin;
        _moveDirection = direction.normalized;
        _speed = moveSpeed;
        _boundX = boxWidth / 2f;
        _boundY = boxHeight / 2f;
        _center = centerPosition;
        
        InstanceFinder.TryGetInstance(out _lobbyConductor);
    }
    
    void Update()
    {
        transform.position += _moveDirection * (_speed * Time.deltaTime);

        Vector3 localPos = transform.position - _center;

        if (Mathf.Abs(localPos.x) > _boundX + 1f || Mathf.Abs(localPos.y) > _boundY + 1f)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        print("Spawned Asteroid");
        Random rnd = new Random();
        int size = rnd.Next(_asteroidObject.AsteroidMinSize, _asteroidObject.AsteroidMaxSize+1);
        gameObject.transform.localScale = Vector3.one * size;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.TryGetComponent(out NetGameplayModule module)) return;
        if (!_bridgeOrigin.IsUnityNull() && _bridgeOrigin == module.Bridge) return;
        
        if (!_lobbyConductor.IsUnityNull())
        {
            if (module.NetTeamID == _netTeamID && _lobbyConductor.FriendlyFireID == NetFirendlyFireID.Off) return;
        }
        
        if (hitModules.Contains(module)) return;
        
        hitModules.Add(module);
        
        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            float friendlyFireDamageMult = 1f;
            if (module.NetTeamID == _netTeamID) friendlyFireDamageMult = _lobbyConductor.FriendlyFireDamageMult;
            
            module.S_InflictDamage(_asteroidObject.AsteroidDamage * friendlyFireDamageMult, _attackerID);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
