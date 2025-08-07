using UnityEngine;
using FishNet;
using System.Collections.Generic;

public class Nebula : MonoBehaviour
{
    [SerializeField] private NebulaObject _nebulaObject;
    [SerializeField] private GameObject hitFeedbackVFX;
    
    private float dt;
    private HashSet<NetGameplayModule> hitModules = new HashSet<NetGameplayModule>();
    
    private Coroutine damageCoroutine;
    
    public void Start()
    {
        float size = Random.Range(_nebulaObject.NebulaMinSize, _nebulaObject.NebulaMaxSize+1);
        gameObject.transform.localScale = Vector3.one * size;
    }

    public void LateUpdate()
    {
        if (InstanceFinder.IsClientStarted) return;
        
        dt += Time.deltaTime;
        
        if (dt > _nebulaObject.NebulaDamageInterval)
        {

            foreach (NetGameplayModule module in hitModules)
            {
                //TODO: Visuals and Audio
                // attacker ID is 0 for neutral damage detection
                module.S_InflictDamage(_nebulaObject.NebulaDamage, 0);
            }
            dt = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (InstanceFinder.IsClientStarted) return;
        if (!other.transform.TryGetComponent(out NetGameplayModule module)) return;
        
        hitModules.Add(module);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (InstanceFinder.IsClientStarted) return;
        if (!other.transform.TryGetComponent(out NetGameplayModule module)) return;
        
        hitModules.Remove(module);
    }
}
