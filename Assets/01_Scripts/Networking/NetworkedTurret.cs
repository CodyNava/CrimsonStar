using FishNet.Object;
using UnityEngine;

public class NetworkedTurret : NetworkBehaviour
{
    [SerializeField] private float cooldown;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private PredictedProjectile projectile;

    private const float MaxPassedTime = 0.3f;

    private Turet _inputAsset;
    private float _accumulatedTime;
    
    public override void OnStartClient()
    {
        if (IsOwner)
        {
            _inputAsset.Enable();
        }
    }

    public override void OnStopClient()
    {
        if (IsOwner)
        {
            _inputAsset.Disable();
        }
    }
    
    private void Awake()
    {
        _inputAsset = new Turet();
    }
    
    private void Update()
    {
        if (!IsOwner) return;
        
        _accumulatedTime += Time.deltaTime;
        _accumulatedTime = Mathf.Min(_accumulatedTime, cooldown);

        if (_accumulatedTime < cooldown) return;
        if (!_inputAsset.Player.Attack.IsPressed()) return;
        
        ClientFire();
        _accumulatedTime -= cooldown;
    }

    private void ClientFire()
    {
        Vector3 position = spawnTransform.position;
        Vector3 direction = spawnTransform.up;
        
        SpawnProjectile(position, direction, 0f);
        ServerFire(position, direction, TimeManager.Tick);
    }

    private void SpawnProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        PredictedProjectile pp = Instantiate(projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.PlayerID);
    }

    [ServerRpc]
    private void ServerFire(Vector3 position, Vector3 direction, uint tick)
    {
        float passedTime = (float) TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        SpawnProjectile(position, direction, passedTime);
        ObserversFire(position, direction, tick);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ObserversFire(Vector3 position, Vector3 direction, uint tick)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime, passedTime);
        SpawnProjectile(position, direction, passedTime);
    }
}
