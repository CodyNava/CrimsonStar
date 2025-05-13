using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetFollowCursor : NetworkBehaviour
{
	private Turet _inputAsset;
	private Plane _plane = new Plane(Vector3.back, Vector3.zero);
	private Camera _camera;
	private Vector2 _input;

	public override void OnStartClient()
	{
		if (IsOwner)
		{
			_camera = Camera.main;
			_inputAsset.Enable();
		}
		else
		{
			enabled = false;
			_inputAsset.Disable();
		}
	}

	private void Awake()
	{
		_inputAsset = new Turet();
	}

	private void Update()
	{
		_input = _inputAsset.Player.Look.ReadValue<Vector2>();
		
		if (_input.magnitude > 0.2f && Gamepad.current != null)
		{
			transform.up = _input.normalized;
		}
		else if (Gamepad.current == null)
		{
			C_LookAtMouse();
		}
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