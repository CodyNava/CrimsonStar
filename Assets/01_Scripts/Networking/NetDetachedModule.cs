using UnityEngine;

namespace _01_Scripts.Networking
{
    public class NetDetachedModule : MonoBehaviour
    {
        [SerializeField] private Vector3 _velocity;
        [SerializeField] private float _velocityDampening;

        private GameObject _visualRoot;

        public void Initialize(NetModuleID moduleID, Vector2 velocity)
        {
            Transform t = transform;
            
            _visualRoot = Instantiate(moduleID.GetModuleData().VisualModelPrefab, t.position, t.rotation);
            _visualRoot.transform.SetParent(t);
            _velocity = new Vector3(velocity.x, velocity.y, 5f);
        }

        public void FixedUpdate()
        {
            _visualRoot.transform.position += _velocity * Time.fixedDeltaTime;
            
            _velocity *= 1f - _velocityDampening / 1000f;

            if (_velocity.sqrMagnitude <= 0.1f)
            {
                MakeStatic();
            }
        }

        private void MakeStatic()
        {
            gameObject.isStatic = true;
            enabled = false;
            
            Debug.Log("Made static!");
        }
    }
}