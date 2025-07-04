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
    [SerializeField] private EventReference shotSound;
    private const float MaxPassedTime = 0.3f;
    private Transform _nextSpawnTransform;
    private VisualEffect _nextMuzzleFlash;
    private float _accumulatedTime;
    private InputAction _fireKey;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            _nextSpawnTransform = spawnTransformA;
        }

        _nextMuzzleFlash = muzzleFlashA;
        SetHotKey(turretModule.WeaponGroup);
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
        Debug.Log("turret"+ _fireKey != null);
        if (_fireKey == null)
        {
            SetHotKey(turretModule.WeaponGroup);
            return;
        }

        if (_fireKey.IsPressed())
        {
            C_ClientFire();
        }

        _accumulatedTime -= netTurretData.Cooldown;
    }

    private void SetHotKey(int group)
    {
        var hotKeyMouse1 = Keybinds.Actions.Player.Attack;
        var hotKeyE = Keybinds.Actions.Player.Attack2;
        var hotKeyQ = Keybinds.Actions.Player.Attack3;
        switch (group)
        {
            case 1:
                _fireKey = hotKeyMouse1;
                break;
            case 2:
                _fireKey = hotKeyE;
                break;
            case 3:
                _fireKey = hotKeyQ;
                break;
        }
    }

    private bool C_IsAttacking()
    {
        if (!InputManager.Instance.IsGamepadUsed)
        {
            return Keybinds.Actions.Player.Attack.IsPressed();
        }
        else
        {
            Vector2 input = Keybinds.Actions.Player.GamepadAim.ReadValue<Vector2>();
            // TODO: The stick deadzone is implemented hardcoded via magic number. Consider to use dedicated Stick deadzone preprocessor in InputActions
            return input.magnitude > 0.2f;
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
        RuntimeManager.PlayOneShot(shotSound, transform.position);
    }

    private void C_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime, ulong senderID)
    {
        NetPredictedProjectile pp = Instantiate(netTurretData.Projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.NetTeamID, senderID);
    }

    [ServerRpc]
    private void S_ServerFire(Vector3 position, Vector3 direction, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        C_SpawnProjectile(position, direction, passedTime, senderID);
        C_ObserversFire(position, direction, tick, senderID);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void C_ObserversFire(Vector3 position, Vector3 direction, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime, passedTime);
        C_SpawnProjectile(position, direction, passedTime, senderID);
        _nextMuzzleFlash.Play();
        RuntimeManager.PlayOneShot(shotSound, transform.position);
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