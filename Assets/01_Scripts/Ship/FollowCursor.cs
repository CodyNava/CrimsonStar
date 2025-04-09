using UnityEngine;

public class FollowCursor : MonoBehaviour
{
	private Plane _plane = new Plane(Vector3.back, Vector3.zero);
	private Camera _camera;
	private void Start()
	{
		_camera = Camera.main;
	}
	private void Update()
	{
		LookAtMouse();
	}
	
	private void LookAtMouse()
	{
		Vector3 mouseScreenPos = Input.mousePosition;
		Ray ray = _camera.ScreenPointToRay(mouseScreenPos);
		_plane.Raycast(ray, out float distance);
	
		Vector3 mouseWorldPos = ray.GetPoint(distance);
		Vector2 direction = (mouseWorldPos - transform.position).normalized;
		transform.up = direction;
	}
}
