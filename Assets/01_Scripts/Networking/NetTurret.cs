using System;
using System.Collections.Generic;
using FishNet.Object;
using FMODUnity;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class NetTurret : NetworkBehaviour
{
    [SerializeField] private NetTurretData netTurretData;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransformA, spawnTransformB;
    [SerializeField] private VisualEffect muzzleFlashA, muzzleFlashB;
    [SerializeField] private StudioEventEmitter shotEvent;
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

    private void LateUpdate()
    {
        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _accumulatedTime = Mathf.Min(_accumulatedTime, netTurretData.Cooldown);

        if (_accumulatedTime < netTurretData.Cooldown) return;

        // if (!C_IsAttacking()) return;

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

        if (!IsHostInitialized)
        {
            C_SpawnProjectile(position, direction, 0f, PlayerData.PlayerID);
        }

        S_ServerFire(position, direction, TimeManager.Tick, PlayerData.PlayerID);
        _nextMuzzleFlash.Play();
        shotEvent.Play();
    }

    private void C_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime, ulong senderID)
    {
        NetPredictedProjectile pp = Instantiate(netTurretData.Projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.NetTeamID, senderID, turretModule.Bridge);
    }

    [ServerRpc]
    private void S_ServerFire(Vector3 position, Vector3 direction, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        if (IsOwner)
        {
            C_SpawnProjectile(position, direction, passedTime, senderID);
        }

        C_ObserversFire(position, direction, tick, senderID);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void C_ObserversFire(Vector3 position, Vector3 direction, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime, passedTime);
        C_SpawnProjectile(position, direction, passedTime, senderID);
        _nextMuzzleFlash.Play();
        shotEvent.Play();
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