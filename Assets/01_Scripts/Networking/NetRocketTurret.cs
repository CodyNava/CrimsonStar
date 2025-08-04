using FishNet.Object;
using FMODUnity;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class NetRocketTurret : NetworkBehaviour
{
    [SerializeField] private NetRocketTurretData netRocketTurretData;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransformA, spawnTransformB;
    [SerializeField] private VisualEffect muzzleFlashA, muzzleFlashB;
    private VisualEffect _nextMuzzleFlash;
    [SerializeField] private StudioEventEmitter shotSound;
    private const float MaxPassedTime = 0.3f;
    private Transform _nextSpawnTransform;
    private float _accumulatedTime;
    private float _cooldownTime;

    
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
        _cooldownTime = Mathf.Min(_accumulatedTime, netRocketTurretData.Cooldown);

        if (_cooldownTime < netRocketTurretData.Cooldown) return;

        //if (!C_IsAttacking()) return;
        if (C_IsAttacking())
        {
            C_ClientFire();
            _accumulatedTime = 0f;
        }
    }

    private bool C_IsAttacking()
    {
        switch (turretModule.WeaponGroup)
        {
            case 2: return Keybinds.Actions.Player.Attack2.IsPressed();
            case 3: return Keybinds.Actions.Player.Attack3.IsPressed();
            default:
            case 1: return Keybinds.Actions.Player.Attack.IsPressed();
        }
    }

    private void C_ClientFire()
    {
        Vector3 position = _nextSpawnTransform.position;
        Vector3 direction = _nextSpawnTransform.up;

        // if (!IsHostInitialized)
        // {
        //     C_SpawnProjectile(position, direction, 0f, PlayerData.PlayerID);
        // }
        if (IsServerInitialized)
        {
            S_SpawnProjectile(position, direction, 0f, PlayerData.PlayerID, turretModule.Bridge);
        }
        else
        {
            S_ServerFire(position, direction, TimeManager.Tick, PlayerData.PlayerID, turretModule.Bridge);
        }
        
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
        
        if (_nextMuzzleFlash == muzzleFlashA)
        {
            _nextMuzzleFlash = muzzleFlashB;
        }
        else
        {
            _nextMuzzleFlash = muzzleFlashA;
        }
        
        _nextMuzzleFlash.Play();
        shotSound.Play();
        
    }

    [Server]
    private void S_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime, ulong senderID,
        NetBridge bridgeOrigin)
    {
        NetPredictedProjectileRocket pp = Instantiate(netRocketTurretData.Projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.NetTeamID, senderID, bridgeOrigin);
        ServerManager.Spawn(pp.gameObject);
    }

    [ServerRpc]
    [Server]
    private void S_ServerFire(Vector3 position, Vector3 direction, uint tick, ulong senderID, NetBridge bridgeOrigin)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        shotSound.Play();
        S_SpawnProjectile(position, direction, passedTime, senderID, bridgeOrigin);
    }
}