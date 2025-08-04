using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField]
    private float _delay = 1f;

    private IEnumerator _destroyDelayCoroutine;

    private void Awake()
    {
        StartCoroutine(DestroyDelayCoroutine());
    }

    private IEnumerator DestroyDelayCoroutine()
    {
        yield return new WaitForSeconds(_delay);
        
        foreach (VisualEffect visualEffect in gameObject.GetComponentsInChildren<VisualEffect>())
        {
            visualEffect.Stop();
        }
        Destroy(gameObject);
        yield return 0f;
    }

}
