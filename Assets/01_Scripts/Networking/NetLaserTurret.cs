using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class NetLaserTurret : NetworkBehaviour
{
    [SerializeField] private NetLaserTurretData netLaserTurretData;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private VisualEffect muzzleCharge, muzzleImpact;
    [SerializeField] private StudioEventEmitter shotSound;
    [SerializeField] private List<Vector4> originalMuzzleChargeColor;


    private const float MaxPassedTime = 0.3f;

    private bool _isCharging;
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
        originalMuzzleChargeColor.Add(muzzleCharge.GetVector4("Color_Blur"));
        originalMuzzleChargeColor.Add(muzzleCharge.GetVector4("Color_LightningBall"));
    }

    private void Update()
    {
        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _cooldownTime = Mathf.Min(_accumulatedTime, netLaserTurretData.Cooldown);

        if (_cooldownTime < netLaserTurretData.Cooldown) return;
        if (!CanFire()) return;


        if (C_IsAttacking())
        {
            _isCharging = true;
            _chargeTime += Time.deltaTime;
            print(_chargeTime);
            muzzleCharge.Play();
            muzzleImpact.Play();
            if (!shotSound.IsPlaying()) shotSound.Play();
            if (_chargeTime >= netLaserTurretData.ChargeTime)
            {
                muzzleCharge.SetVector4("Color_Blur", Color.red * 6);
                muzzleCharge.SetVector4("Color_LightningBall", Color.red * 6);
            }
        }
        else
        {
            if (_chargeTime >= netLaserTurretData.ChargeTime)
            {
                _isCharging = false;
                _accumulatedTime = 0;
                _chargeTime = 0;
                _cooldownTime = 0;
                C_ClientFire();
                StartCoroutine(ColorChanger());
                return;
            }

            _isCharging = false;
            muzzleCharge.Stop();
            muzzleImpact.Stop();
            _chargeTime = Mathf.Lerp(_chargeTime, 0, Time.deltaTime);
            if (shotSound.IsPlaying()) shotSound.Stop();
        }
    }

    private IEnumerator ColorChanger()
    {
        yield return new WaitForSeconds(muzzleCharge.GetFloat("Get_ChargeTime") / 2);
        muzzleCharge.SetVector4("Color_Blur", originalMuzzleChargeColor[0]);
        muzzleCharge.SetVector4("Color_LightningBall", originalMuzzleChargeColor[1]);
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
        Vector3 position = spawnTransform.position;
        Vector3 direction = spawnTransform.up;

        if (!IsHostInitialized)
        {
            C_SpawnProjectile(position, direction, 0f, PlayerData.PlayerID);
        }

        S_ServerFire(position, direction, TimeManager.Tick, PlayerData.PlayerID);
    }

    private void C_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime, ulong senderID)
    {
        NetPredictedProjectileLaser pp = Instantiate(netLaserTurretData.Projectile, position, Quaternion.identity);
        pp.transform.SetParent(this.transform); 
        pp.Initialize(direction, passedTime, turretModule.NetTeamID, senderID, turretModule.Bridge, spawnTransform.transform);
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
        muzzleCharge.Play();
        shotSound.Play();
    }
}