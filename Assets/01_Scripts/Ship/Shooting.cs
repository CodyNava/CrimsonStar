using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using UnityEngine;



public class Shooting : MonoBehaviour
{
    [SerializeField] private Projectile bullet;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;
    [SerializeField] private float cooldown;
    private float accumulatedTime;
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
        SpawnObject(spawn1);
        SpawnObject(spawn2);
    }
    void SpawnObject(Transform spawn)
    {
        Projectile projectile = Instantiate(bullet, spawn.position, Quaternion.identity);
        Vector3 shootDirection = transform.up;

        projectile.source = transform.root;
        projectile.direction = shootDirection;
        projectile.speed = projectileSpeed;
        projectile.lifetime = projectileLifetime;
    }
}
