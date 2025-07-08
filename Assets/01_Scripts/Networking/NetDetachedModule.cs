using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts.Networking
{
    public class NetDetachedModule : MonoBehaviour
    {
        [Header("Physics Settings")]
        [Tooltip("Multiplies the velocity x and y component")]
        [SerializeField] private Vector2 _velocityVariation;
        [Tooltip("Multiplies the velocity magnitude")]
        [SerializeField] private Vector2 _velocityMagnitudeVariation;
        [SerializeField] private float _defaultMagnitude;
        [SerializeField] private float _depthVelocity;
        [SerializeField] private float _velocityDampening;

        [Header("Decay Settings")] [SerializeField]
        private bool _decayAfterTime;

        [SerializeField] private float _decayTime;
        [Tooltip("Multiplies the decay time")]
        [SerializeField] private Vector2 _decayVariation;
        [SerializeField] private GameObject deathVFX;
        
        private Vector3 _velocity;
        private GameObject _visualRoot;
        private float _elapsedTime = 0f;

        public void Initialize(NetModuleID moduleID, Vector2 velocity, float magnitude)
        {
            Transform t = transform;

            if (velocity.sqrMagnitude > Vector2.one.sqrMagnitude)
            {
                velocity.Normalize();
            }

            float velVarX = Random.Range(_velocityVariation.x, _velocityVariation.y);
            float velVarY = Random.Range(_velocityVariation.x, _velocityVariation.y);
            float velVarZ = Random.Range(_velocityVariation.x, _velocityVariation.y);
            float magnitudeVariation = Random.Range(_velocityMagnitudeVariation.x, _velocityMagnitudeVariation.y);
            magnitude *= magnitudeVariation;
            
            float decayVar = Random.Range(_decayVariation.x, _decayVariation.y);
            _decayTime *= decayVar;

            _visualRoot = Instantiate(moduleID.GetModuleData().VisualModelPrefab, t.position, t.rotation);
            _visualRoot.transform.SetParent(t);
            
            _velocity = new Vector3(
                velocity.x * velVarX * magnitude, 
                velocity.y * velVarY * magnitude, 
                _depthVelocity * velVarZ
            );
        }

        public void Initialize(NetModuleID moduleID, Vector2 velocity)
        {
            Initialize(moduleID, velocity, _defaultMagnitude);
        }

        public void Update()
        {
            if (!_decayAfterTime) return;
            
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _decayTime)
            {
                //Instantiate(deathVFX, _visualRoot.transform.position, Quaternion.identity);
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