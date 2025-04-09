using UnityEngine;

namespace _01_Scripts.AI
{
    public class EnemyMovementController : MonoBehaviour
    {
        [SerializeField] private float maxAngularAcceleration = 1f;
        [SerializeField] private float maxAcceleration = .2f;
        [SerializeField] private float maxVelocity = 10f;

        private Vector2 _moveTowards = Vector2.zero;
        public Vector2 MoveTowards
        {
            get => _moveTowards;
            set => _moveTowards = value.normalized * Mathf.Min(value.magnitude, 1f);
        }
        

        private Vector2 _velocity = Vector2.zero;
        
        public void Update()
        {
            Transform t = transform;
            
            float lookingTowardsMovement = Vector2.Dot(t.up, _moveTowards);
            float lookRotationFactor = 1 - ((lookingTowardsMovement + 1f) * 0.25f);
            float lookSpeedFactor = Mathf.Max(lookingTowardsMovement, 0.15f);
            

            Vector2 desiredVelocity =  maxVelocity * lookSpeedFactor * _moveTowards;
            float maxSpeedChange = maxAcceleration * Time.deltaTime;
            _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
            _velocity.y = Mathf.MoveTowards(_velocity.y, desiredVelocity.y, maxSpeedChange);
            
            Vector2 accel = MoveTowards * maxAcceleration;

            _velocity += accel * Time.deltaTime;
            transform.localPosition += (Vector3)_velocity * Time.deltaTime;

            if (MoveTowards.sqrMagnitude > 0.001f)
            {
                float currentRotation = t.rotation.eulerAngles.z;

                float maxRotationSpeedChange = lookRotationFactor * maxAngularAcceleration * Time.deltaTime;
                float desiredAngle = (Mathf.Atan2(_velocity.x, _velocity.y) * Mathf.Rad2Deg) * (-1);
                float angle =
                    Mathf.MoveTowardsAngle(currentRotation, desiredAngle, maxRotationSpeedChange);

                transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            }

        }

        public void OnDrawGizmosSelected()
        {
            if (_velocity.sqrMagnitude > 0.01f)
            {
                Vector3 pos = transform.position;
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pos + (Vector3)_moveTowards, 0.1f);
                Gizmos.DrawLine(pos, pos + (Vector3)_moveTowards);
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(pos + (Vector3)_velocity, 0.1f);
                Gizmos.DrawLine(pos, pos + (Vector3)_velocity);
            } 
        }
    }
}