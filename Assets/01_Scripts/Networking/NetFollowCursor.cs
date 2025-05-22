using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetFollowCursor : NetworkBehaviour
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
		if (InputManager.Instance.IsGamepadUsed)
		{
			_input = Keybinds.Actions.Player.GamepadAim.ReadValue<Vector2>();
			
			// TODO: The stick deadzone is implemented hardcoded via magic number. Consider to use dedicated Stick deadzone preprocessor in InputActions
			if (_input.magnitude <= 0.2f) return;

			transform.up = _input.normalized;
		}
		else if (!InputManager.Instance.IsGamepadUsed)
		{
			_input = Keybinds.Actions.Player.MouseAim.ReadValue<Vector2>();
			C_LookAtMouse();
		}
	}
	
	private void C_LookAtMouse()
	{
		if (!_camera) return;
		
		Ray ray = _camera.ScreenPointToRay(_input);
		_plane.Raycast(ray, out float distance);
	
		Vector3 mouseWorldPos = ray.GetPoint(distance);
		Vector2 direction = (mouseWorldPos - transform.position).normalized;
		transform.up = direction;
	}
}