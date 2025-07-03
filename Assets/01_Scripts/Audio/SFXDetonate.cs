using UnityEngine;

public class SFXDetonate : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter ExplosionEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot(detonateSound, transform.position);
    }
}