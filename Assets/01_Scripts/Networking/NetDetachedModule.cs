using System;
using UnityEngine;

namespace _01_Scripts.Networking
{
    public class NetDetachedModule : MonoBehaviour
    {
        [Header("Physics Settings")]
        [SerializeField] private float _velocityDampening;

        [Header("Decay Settings")] [SerializeField]
        private bool _decayAfterTime;

        [SerializeField] private float _decayTime;
        [SerializeField] private GameObject deathVFX;
        
        private Vector3 _velocity;
        private GameObject _visualRoot;
        private float _elapsedTime = 0f;

        public void Initialize(NetModuleID moduleID, Vector2 velocity)
        {
            Transform t = transform;
            
            _visualRoot = Instantiate(moduleID.GetModuleData().VisualModelPrefab, t.position, t.rotation);
            _visualRoot.transform.SetParent(t);
            _velocity = new Vector3(velocity.x, velocity.y, 5f);
        }

        public void Update()
        {
            if (!_decayAfterTime) return;
            
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _decayTime)
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        public void FixedUpdate()
        {
            _visualRoot.transform.position += _velocity * Time.fixedDeltaTime;
            
            _velocity *= 1f - _velocityDampening / 1000f;

            if (!_decayAfterTime && _velocity.sqrMagnitude <= 0.1f)
            {
                MakeStatic();
            }
        }

        private void MakeStatic()
        {
            gameObject.isStatic = true;
            enabled = false;
        }
    }
}