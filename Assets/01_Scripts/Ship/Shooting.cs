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
    [SerializeField] AudioClip shotSound;
    [SerializeField] AudioSource audioSource;
    private float accumulatedTime;
    private bool canShoot;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 3f;

    public bool _triggerShot;

    protected virtual void Update()
    {
        accumulatedTime += Time.deltaTime;
        accumulatedTime = Mathf.Clamp(accumulatedTime, 0f, cooldown);

        if (!_triggerShot) return;
        if (!(accumulatedTime >= cooldown)) return;

        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(shotSound);
        Shoot();
        _triggerShot = false;
        accumulatedTime -= cooldown * Random.Range(0.95f, 1f);
    }

    protected virtual void Awake()
    {
        Combat_GameState.onEnterState += OnEnterCombatState;
        Combat_GameState.onExitState += OnExitCombatState;
    }

    public virtual void OnDestroy()
    {
        Combat_GameState.onEnterState -= OnEnterCombatState;
        Combat_GameState.onExitState -= OnExitCombatState;
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