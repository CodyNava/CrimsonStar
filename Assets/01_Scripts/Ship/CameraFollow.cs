using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _target;
    [SerializeField] private Camera cam;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Vector3 offset;
    private Vector3 velocity = Vector3.zero;


    void LateUpdate()
    {
        if (_target != null)
        {
            Vector3 targetPos = _target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            return;
        }
    }
    public void SetTargetFollow(Transform target)
    {
        _target = target;
    }
}
