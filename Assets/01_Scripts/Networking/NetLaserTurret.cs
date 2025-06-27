using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.VFX; 
public class NetLaserTurret : NetworkBehaviour
{
    [SerializeField] private NetLaserTurretData netLaserTurretData;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private VisualEffect muzzleCharge, muzzleImpact;
    [SerializeField] private AudioSource shootingSound;
    [SerializeField] private bool isCharging;
    
    private const float MaxPassedTime = 0.3f;

    private float _accumulatedTime;
    private float _cooldownTime;
    private float _chargeTime;
    private bool _justShot;


    private bool CanFire()
    {
        return turretModule.Bridge.PositionHasEnergy(turretModule.RootCoordinate);
    }

    private void Start()
    {
        muzzleCharge.SetFloat("Get_ChargeTime", netLaserTurretData.ChargeTime);
        muzzleImpact.SetFloat("Delay", netLaserTurretData.ChargeTime);
    }
    private void Update()
    {
        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _cooldownTime = Mathf.Min(_accumulatedTime, netLaserTurretData.Cooldown);
        

        if (_cooldownTime < netLaserTurretData.Cooldown) return;
        if (!CanFire()) return;
       // if (!C_IsAttacking()) return;
        
        if (Keybinds.Actions.Player.Attack.IsPressed() || isCharging)
        {
            isCharging = true;
            _chargeTime += Time.deltaTime;
            print(_chargeTime);
            muzzleCharge.Play();
            muzzleImpact.Play();
            if (_chargeTime >= netLaserTurretData.ChargeTime)
            {
                _accumulatedTime = 0;
                _chargeTime = 0;
                _cooldownTime = 0f;
                isCharging = false;
                C_ClientFire();
            }
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
        Vector3 position = spawnTransform.position;
        Vector3 direction = spawnTransform.up;

        if (!IsHostInitialized)
        {
            C_SpawnProjectile(position, direction, 0f, PlayerData.PlayerID);
        }
        S_ServerFire(position, direction, TimeManager.Tick, PlayerData.PlayerID);
        shootingSound.Play();
    }

    private void C_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime, ulong senderID)
    {
        print("Spawning projectile");
        NetPredictedProjectileLaser pp = Instantiate(netLaserTurretData.Projectile, position, Quaternion.identity);
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
        muzzleCharge.Play();
    }
}