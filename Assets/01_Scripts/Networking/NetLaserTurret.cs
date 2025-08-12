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
    [SerializeField] private VisualEffect muzzleCharge;
    [SerializeField] private StudioEventEmitter chargeSound;

    [SerializeField] private NetModuleData moduleData;
    [SerializeField] private List<HexCoordinate> localCoordinates;


    private const float MaxPassedTime = 0.3f;

    private bool _isCharging;
    private float _accumulatedTime;
    private float _accumulatedTimeVFX;
    private float _cooldownTime;
    private float _chargeTime;
    private float _chargingValue;
    private bool _justShot;


    private bool CanFire()
    {
        if (!this.turretModule.ModuleID.GetModuleData().CanBePowered) return true;
        return turretModule.Bridge.PositionHasEnergy(turretModule.ModuleCoordinates);
    }
    
    private void Start()
    {
        muzzleCharge.SetFloat("Get_ChargeTime", netLaserTurretData.ChargeTime);
        chargeSound.EventInstance.getParameterByName("laser-charging", out _chargingValue);
        // S_ServerStartSFX();
        if (!chargeSound.IsPlaying()) chargeSound.Play();
        //muzzleImpact.SetFloat("Delay", netLaserTurretData.ChargeTime);
    }

    private void Update()
    {
        if (!chargeSound.IsPlaying()) chargeSound.Play();

        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _cooldownTime = Mathf.Min(_accumulatedTime, netLaserTurretData.Cooldown);

        if (_cooldownTime < netLaserTurretData.Cooldown) return;
        


        if (C_IsAttacking())
        {
            if (!CanFire()) return;
            chargeSound.EventInstance.getParameterByName("laser-charging", out _chargingValue);

            S_ServerSFXChargeUp();
            _accumulatedTimeVFX += Time.deltaTime;
            if (!_isCharging && _accumulatedTimeVFX > 0.3f)
            {
                _isCharging = true;
                S_ServerVFXString("ChargeUp");
            }


            _chargeTime += Time.deltaTime;
            if (_chargeTime >= netLaserTurretData.ChargeTime)
            {
                S_ServerVFXBool(true);
                _accumulatedTimeVFX = 0f;
            }
        }
        else
        {
            if (_chargeTime >= netLaserTurretData.ChargeTime)
            {
                chargeSound.EventInstance.getParameterByName("laser-charging", out _chargingValue);


                _isCharging = false;
                _accumulatedTime = 0;
                _chargeTime = 0;
                _cooldownTime = 0;
                _accumulatedTimeVFX = 0f;
                C_ClientFire();
                chargeSound.Stop();
                if (!chargeSound.IsPlaying()) chargeSound.Play();
                //S_ServerSFXLaserShoot();
                S_ServerVFXBool(false);
                S_ServerVFXString("StartDissolve");
                return;
            }

            chargeSound.EventInstance.getParameterByName("laser-charging", out _chargingValue);
            S_ServerSFXChargeDown();

            if (_isCharging)
            {
                S_ServerVFXString("ChargeDown");
            }

            _isCharging = false;
            _chargeTime = Mathf.Lerp(_chargeTime, 0, Time.deltaTime * 2);
            _accumulatedTimeVFX = _chargeTime;
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
        pp.Initialize(direction, passedTime, turretModule.NetTeamID, senderID, turretModule.Bridge,
            spawnTransform.transform);
    }

    [ServerRpc]
    private void S_ServerFire(Vector3 position, Vector3 direction, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        if (IsOwner)
        {
            C_SpawnProjectile(position, direction, passedTime, senderID);
            chargeSound.EventInstance.setParameterByName("laser-charging", 0);
            chargeSound.EventInstance.setParameterByName("laser-hold", 0);
        }

        C_ObserversFire(position, direction, tick, senderID);
    }


    [ObserversRpc(ExcludeOwner = true)]
    private void C_ObserversFire(Vector3 position, Vector3 direction, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime, passedTime);

        chargeSound.EventInstance.setParameterByName("laser-charging", 0);
        chargeSound.EventInstance.setParameterByName("laser-hold", 0);

        C_SpawnProjectile(position, direction, passedTime, senderID);
    }

    [ServerRpc]
    private void S_ServerVFXBool(bool vfxBool)
    {
        C_ObserverVFXBool(vfxBool);
    }

    [ServerRpc]
    private void S_ServerVFXString(string vfxString)
    {
        C_ObserverVFXString(vfxString);
    }

    [ObserversRpc]
    private void C_ObserverVFXBool(bool vfxBool)
    {
        muzzleCharge.SetBool("Set_ChargeCompleted", vfxBool);
    }

    [ObserversRpc]
    private void C_ObserverVFXString(string vfxString)
    {
        switch (vfxString)
        {
            case "ChargeUp":
                muzzleCharge.SendEvent(vfxString);
                break;
            case "StartDissolve":
                muzzleCharge.SendEvent(vfxString);
                break;
            case "ChargeDown":
                muzzleCharge.SendEvent(vfxString);
                break;
        }
    }


    [ServerRpc]
    private void S_ServerSFXChargeUp()
    {
        C_ObserverSFXChargeUp();
    }

    [ServerRpc]
    private void S_ServerSFXChargeDown()
    {
        C_ObserverSFXChargeDown();
    }

    [ServerRpc]
    private void S_ServerSFXLaserShoot()
    {
        C_ObserverSFXLaserShoot();
    }

    [ObserversRpc]
    private void C_ObserverSFXLaserShoot()
    {
        if (_chargingValue == 0)
        {
            return;
        }
        else
        {
            chargeSound.EventInstance.setParameterByName("laser-charging", 0);
            chargeSound.EventInstance.setParameterByName("laser-hold", 0);
        }
    }

    [ObserversRpc]
    private void C_ObserverSFXChargeUp()
    {
        if (_chargingValue == 1)
        {
            return;
        }
        else
        {
            chargeSound.EventInstance.setParameterByName("laser-charging", 1);
            chargeSound.EventInstance.setParameterByName("laser-hold", 1);
        }
    }

    [ObserversRpc]
    private void C_ObserverSFXChargeDown()
    {
        if (_chargingValue == 2)
        {
            return;
        }
        else
        {
            chargeSound.EventInstance.setParameterByName("laser-charging", 2);
            chargeSound.EventInstance.setParameterByName("laser-hold", 0);
        }
    }

    private void OnDestroy()
    {
        chargeSound.Stop();
    }
}