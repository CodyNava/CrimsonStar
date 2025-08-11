using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Random = UnityEngine.Random;

public class AsteroidSpawner : NetworkBehaviour
{
    [SerializeField] private AsteroidObject _asteroidObject;
    private readonly SyncVar<NetTeamID> _netTeamID = new SyncVar<NetTeamID>();
    private ulong _attackerID = 0;
    
    private Vector3 spawnPos = Vector3.zero;
    private Vector3 baseDir = Vector3.zero;
    private float timer;
    private float speed;
    private Vector3 finalDirection;

    private void Start()
    {
        _netTeamID.Value = NetTeamID.Environment;
        _attackerID = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= _asteroidObject.AsteroidSpawnInterval)
        {
            timer = 0f;
            SpawnAsteroid();
        }
    }
    
    public void Initialize(float passedTime, NetTeamID netTeamID, ulong attackerID = 0)
    {
        _netTeamID.Value = netTeamID;
        _attackerID = attackerID;
    }
    
    void SpawnAsteroid()
    {
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // Top (Y+)
                spawnPos = new Vector3(Random.Range(-_asteroidObject.AsteroidSpawnerWidth / 2, _asteroidObject.AsteroidSpawnerWidth / 2), _asteroidObject.AsteroidSpawnerHeight / 2, 0);
                baseDir = Vector3.down;
                break;
            case 1: // Bottom (Y-)
                spawnPos = new Vector3(Random.Range(-_asteroidObject.AsteroidSpawnerWidth / 2, _asteroidObject.AsteroidSpawnerWidth / 2), -_asteroidObject.AsteroidSpawnerHeight / 2, 0);
                baseDir = Vector3.up;
                break;
            case 2: // Left (X-)
                spawnPos = new Vector3(-_asteroidObject.AsteroidSpawnerWidth / 2, Random.Range(-_asteroidObject.AsteroidSpawnerHeight / 2, _asteroidObject.AsteroidSpawnerHeight / 2), 0);
                baseDir = Vector3.right;
                break;
            case 3: // Right (X+)
                spawnPos = new Vector3(_asteroidObject.AsteroidSpawnerWidth / 2, Random.Range(-_asteroidObject.AsteroidSpawnerHeight / 2, _asteroidObject.AsteroidSpawnerHeight / 2), 0);
                baseDir = Vector3.left;
                break;
        }

        float angleOffset = Random.Range(-30f, 30f);
        finalDirection = Quaternion.Euler(0, 0, angleOffset) * baseDir;

        speed = Random.Range(_asteroidObject.AsteroidMinSpeed, _asteroidObject.AsteroidMaxSpeed);

        Asteroid asteroid = Instantiate(_asteroidObject.AsteroidPrefab, transform.position + spawnPos, Quaternion.identity);
        asteroid.Initialize(_netTeamID.Value, _asteroidObject, finalDirection, speed, _asteroidObject.AsteroidSpawnerWidth, _asteroidObject.AsteroidSpawnerHeight, transform.position);
        ServerManager.Spawn(asteroid.gameObject, null);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(_asteroidObject.AsteroidSpawnerWidth, _asteroidObject.AsteroidSpawnerHeight, 0.1f));
    }
}
