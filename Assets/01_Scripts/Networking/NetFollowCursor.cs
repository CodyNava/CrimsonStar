using FishNet.Object;
using UnityEngine;

public class NetFollowCursor : NetworkBehaviour
{
    private Plane _plane = new Plane(Vector3.back, Vector3.zero);
    private Camera _camera;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            _camera = Camera.main;
        }
        else
        {
            enabled = false;
        }
    }

    private void Update()
    {
        C_LookAtMouse();
    }
	
    private void C_LookAtMouse()
    {
        if (!_camera) return;
        
        Vector3 mouseScreenPos = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(mouseScreenPos);
        _plane.Raycast(ray, out float distance);
	
        Vector3 mouseWorldPos = ray.GetPoint(distance);
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        transform.up = direction;
    }
}
