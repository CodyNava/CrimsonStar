using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using UnityEngine;
using UnityEngine.Serialization;


public class Shooting : MonoBehaviour
{
    [SerializeField] private Projectile bullet;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;
    [SerializeField] private ParticleSystem muzzleFlash1;
    [SerializeField] private ParticleSystem muzzleFlash2;
    [SerializeField] private float cooldown;
    [SerializeField] private LayerMask projectileLayerMask;
    private float accumulatedTime;
    private bool canShoot;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 3f;

    private Turet playerInput;

    private void Update()
    {
        accumulatedTime += Time.deltaTime;
        accumulatedTime = Mathf.Clamp(accumulatedTime, 0f, cooldown);
        if (playerInput.Player.Attack.IsPressed())
        {
            if (accumulatedTime >= cooldown)
            {
                Shoot();
                accumulatedTime -= cooldown * Random.Range(0.95f, 1f);
            }
        }
    }
    private void Awake()
    {
        Combat_GameState.onEnterState -= OnEnterCombatState;
        Combat_GameState.onExitState -= OnExitCombatState;
        Combat_GameState.onEnterState += OnEnterCombatState;
        Combat_GameState.onExitState += OnExitCombatState;
        playerInput = new Turet();
        playerInput.Enable();
    }

    public void OnDestroy()
    {
        Combat_GameState.onEnterState -= OnEnterCombatState;
        Combat_GameState.onExitState -= OnExitCombatState;
        playerInput.Disable();
    }
    private void OnExitCombatState()
    {
        enabled = false;
    }

    private void OnEnterCombatState(GameStateController controller)
    {
        enabled = true;
    }

    public void Shoot()
    {
        muzzleFlash1.Play();
        muzzleFlash2.Play();

        SpawnObject(spawn1);
        SpawnObject(spawn2);
    }
    void SpawnObject(Transform spawn)
    {
        Projectile projectile = Instantiate(bullet, spawn.position, Quaternion.identity);
        Vector3 shootDirection = transform.up;

        projectile.gameObject.layer = (int)Mathf.Log(projectileLayerMask.value, 2f);
        projectile.source = transform.root;
        projectile.direction = shootDirection;
        projectile.speed = projectileSpeed;
        projectile.lifetime = projectileLifetime;
    }
}
