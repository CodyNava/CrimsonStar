using FishNet.Object;
using UnityEngine;
using UnityEngine.VFX;

public class NetTurret : NetworkBehaviour
{
    [SerializeField] private NetTurretData netTurretData;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransformA, spawnTransformB;
    [SerializeField] private VisualEffect muzzleFlashA, muzzleFlashB;

    private const float MaxPassedTime = 0.3f;
    private Transform _nextSpawnTransform;
    private VisualEffect _nextMuzzleFlash;

    private Turet _inputAsset;
    private float _accumulatedTime;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            _inputAsset.Enable();
            _nextSpawnTransform = spawnTransformA;
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
        _accumulatedTime = Mathf.Min(_accumulatedTime, netTurretData.Cooldown);

        if (_accumulatedTime < netTurretData.Cooldown) return;
        if (!_inputAsset.Player.Attack.IsPressed()) return;

        if (_nextSpawnTransform == spawnTransformA)
        {
            _nextSpawnTransform = spawnTransformB;
            _nextMuzzleFlash = muzzleFlashB;
        }
        else
        {
            _nextSpawnTransform = spawnTransformA;
            _nextMuzzleFlash = muzzleFlashA;
        }

        ClientFire();
        _accumulatedTime -= netTurretData.Cooldown;
    }

    private void ClientFire()
    {
        Vector3 position = _nextSpawnTransform.position;
        Vector3 direction = _nextSpawnTransform.up;

        SpawnProjectile(position, direction, 0f);
        ServerFire(position, direction, TimeManager.Tick);
        _nextMuzzleFlash.Play();
    }

    private void SpawnProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        NetPredictedProjectile pp = Instantiate(netTurretData.Projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.NetPlayerID);
    }

    [ServerRpc]
    private void ServerFire(Vector3 position, Vector3 direction, uint tick)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
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
        _nextMuzzleFlash.Play();
    }
}
