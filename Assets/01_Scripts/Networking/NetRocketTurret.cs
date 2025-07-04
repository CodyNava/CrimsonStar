using FishNet.Object;
using FMODUnity;
using UnityEngine;
using UnityEngine.VFX;

public class NetRocketTurret : NetworkBehaviour
{
    [SerializeField] private NetRocketTurretData netRocketTurretData;
    [SerializeField] private NetGameplayModule turretModule;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private VisualEffect muzzleFlash;
    [SerializeField] private StudioEventEmitter shotSound;
    private const float MaxPassedTime = 0.3f;


    private float _accumulatedTime;
    private float _cooldownTime;

    private void Update()
    {
        if (!IsOwner) return;
        _accumulatedTime += Time.deltaTime;
        _cooldownTime = Mathf.Min(_accumulatedTime, netRocketTurretData.Cooldown);

        if (_cooldownTime < netRocketTurretData.Cooldown) return;

        if (!C_IsAttacking()) return;

        C_ClientFire();

        _accumulatedTime = 0f;
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

        // if (!IsHostInitialized)
        // {
        //     C_SpawnProjectile(position, direction, 0f, PlayerData.PlayerID);
        // }
        if (IsServerInitialized)
        {
            S_SpawnProjectile(position, direction, 0f, PlayerData.PlayerID);
        }
        else
        {
            S_ServerFire(position, direction, TimeManager.Tick, PlayerData.PlayerID);
        }

        muzzleFlash.Play();
        shotSound.Play();
    }

    [Server]
    private void S_SpawnProjectile(Vector3 position, Vector3 direction, float passedTime, ulong senderID)
    {
        NetPredictedProjectileRocket pp = Instantiate(netRocketTurretData.Projectile, position, Quaternion.identity);
        pp.Initialize(direction, passedTime, turretModule.NetTeamID, senderID);
        ServerManager.Spawn(pp.gameObject);
    }

    [ServerRpc]
    [Server]
    private void S_ServerFire(Vector3 position, Vector3 direction, uint tick, ulong senderID)
    {
        float passedTime = (float)TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MaxPassedTime / 2f, passedTime);

        shotSound.Play();
        S_SpawnProjectile(position, direction, passedTime, senderID);
    }
}