using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetFollowCursorLaser : NetworkBehaviour
{
	private Plane _plane = new Plane(Vector3.back, Vector3.zero);
	private Camera _camera;
	private Vector2 _input;

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
		_input = Keybinds.Actions.Player.Look.ReadValue<Vector2>();
		
		if (_input.magnitude > 0.2f && InputManager.Instance.IsGamepadUsed)
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
		if (Keybinds.Actions.Player.Attack.IsPressed()) return;
		Ray ray = _camera.ScreenPointToRay(_input);
		_plane.Raycast(ray, out float distance);
	
		Vector3 mouseWorldPos = ray.GetPoint(distance);
		Vector2 direction = (mouseWorldPos - transform.position).normalized;
		transform.up = direction;
	}
}