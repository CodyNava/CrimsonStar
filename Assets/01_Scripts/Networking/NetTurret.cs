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

    private float _accumulatedTime;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            _nextSpawnTransform = spawnTransformA;
        }

        _nextMuzzleFlash = muzzleFlashA;
    }

    private void Update()
    {
        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _accumulatedTime = Mathf.Min(_accumulatedTime, netTurretData.Cooldown);

        if (_accumulatedTime < netTurretData.Cooldown) return;
        if (!Keybinds.Actions.Player.Attack.IsPressed()) return;

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

        C_ClientFire();
        _accumulatedTime -= netTurretData.Cooldown;
    }

    private void C_ClientFire()
    {
        Vector3 position = _nextSpawnTransform.position;
        Vector3 direction = _nextSpawnTransform.up;

        C_SpawnProjectile(position, direction, 0f);
        S_ServerFire(position, direction, TimeManager.Tick);
        _nextMuzzleFlash.Play();
    }

    private void C_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        NetPredictedProjectile pp = Instantiate(netTurretData.Projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.NetPlayerID);
    }

    [ServerRpc]
    private void S_ServerFire(Vector3 position, Vector3 direction, uint tick)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        C_SpawnProjectile(position, direction, passedTime);
        C_ObserversFire(position, direction, tick);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void C_ObserversFire(Vector3 position, Vector3 direction, uint tick)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime, passedTime);
        C_SpawnProjectile(position, direction, passedTime);
        _nextMuzzleFlash.Play();

        if (_nextMuzzleFlash == muzzleFlashA)
        {
            _nextMuzzleFlash = muzzleFlashB;
        }
        else
        {
            _nextMuzzleFlash = muzzleFlashA;
        }
    }
}