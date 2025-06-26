using System.Collections.Generic;
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
    

    private bool CanFire()
    {
        Debug.Log($"PowerGrid: {turretModule.Bridge.PowerGrid.Count}");
        foreach (KeyValuePair<HexCoordinate,int> gridEntry in turretModule.Bridge.PowerGrid)
        {
            Debug.Log($"PoweredCoords: [{gridEntry.Key.Q},{gridEntry.Key.R},{gridEntry.Key.S}]: {gridEntry.Value}");
        }
        return turretModule.Bridge.PositionHasEnergy(turretModule.RootCoordinate);
    }
   
    private void Update()
    {
        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _cooldownTime = Mathf.Min(_accumulatedTime, netLaserTurretData.Cooldown);
        

        if (_cooldownTime < netLaserTurretData.Cooldown) return;
        if (!CanFire()) return;
       // if (!C_IsAttacking()) return;
        
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
        muzzleFlash.Play();
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
        muzzleFlash.Play();
    }
}