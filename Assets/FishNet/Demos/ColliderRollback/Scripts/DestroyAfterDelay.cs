using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace FishNet.Example.ColliderRollbacks
{
    public class DestroyAfterDelay : MonoBehaviour
    {
        [SerializeField]
        private float _delay = 1f;

        [SerializeField] private List<VisualEffect> _visualEffects = new List<VisualEffect>();

        private void Awake()
        {
            foreach (VisualEffect visualEffect in _visualEffects)
            {
                visualEffect.Stop();
            }
            Destroy(gameObject, _delay);
        }

    }

}