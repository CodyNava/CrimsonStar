using FishNet.Object;
using UnityEngine;
using UnityEngine.VFX; 
public class NetLaserTurret : NetworkBehaviour
{
    [SerializeField] private NetLaserTurretData netLaserTurretData;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private VisualEffect muzzleFlash;
    [SerializeField] private AudioSource shootingSound;
    private const float MaxPassedTime = 0.3f;

    

    private float _accumulatedTime;
    private float _cooldownTime;
    private float _chargeTime;
    private bool _justShot;
    

    private void Update()
    {
        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _cooldownTime = Mathf.Min(_accumulatedTime, netLaserTurretData.Cooldown);
        

        if (_cooldownTime < netLaserTurretData.Cooldown) return;

        if (Keybinds.Actions.Player.Attack.IsPressed())
        {
            _chargeTime += Time.deltaTime;
            print(_chargeTime);
            if (_chargeTime >= netLaserTurretData.ChargeTime)
            {
                _accumulatedTime = 0;
                _chargeTime = 0;
                _cooldownTime = 0f;
                C_ClientFire();
            }
        }
        else
        {
            _chargeTime = 0;
        }
    }

    private void C_ClientFire()
    {
        Vector3 position = spawnTransform.position;
        Vector3 direction = spawnTransform.up;

        if (!IsHostInitialized)
        {
            C_SpawnProjectile(position, direction, 0f);
        }
        S_ServerFire(position, direction, TimeManager.Tick);
        muzzleFlash.Play();
        shootingSound.Play();
    }

    private void C_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        print("Spawning projectile");
        NetPredictedProjectileLaser pp = Instantiate(netLaserTurretData.Projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.NetTeamID);
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
        muzzleFlash.Play();
    }
}