using UnityEngine;
using FishNet;
using Random = System.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private AsteroidObject _asteroidObject;
    [SerializeField] private GameObject hitFeedbackVFX;
    


    public void Start()
    {
        Random rnd = new Random();
        int size = rnd.Next(_asteroidObject.AsteroidMinSize, _asteroidObject.AsteroidMaxSize+1);
        gameObject.transform.localScale = Vector3.one * size;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent(out NetGameplayModule module)) return;
        
        
        if (InstanceFinder.IsClientStarted)
        {
            // Visual and Audio
        }

        if (InstanceFinder.IsServerStarted)
        {
            module.S_InflictDamage(_asteroidObject.AsteroidDamage, 0);
        }
        Instantiate(hitFeedbackVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
