using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class Shooting : MonoBehaviour
{	
	[SerializeField] private Projectile bullet;
	[SerializeField] private Transform spawn1;
	[SerializeField] private Transform spawn2;
	public float projectileSpeed = 10f;
	public float projectileLifetime = 3f;
	
	private Turet playerInput;

	private void Awake()
	{
		playerInput = new Turet();
		playerInput.Enable();
	}

	private void OnEnable()
	{
		playerInput.Player.Attack.performed += OnShoot;
		// playerInput.Player.Attack.Enable();
	}

	void OnDisable()
	{
		playerInput.Player.Attack.performed -= OnShoot;
		// playerInput.Player.Attack.Disable();
	}

	public void OnShoot(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			SpawnObject(spawn1);
			SpawnObject(spawn2);
		}
	}

	void SpawnObject(Transform spawn)
	{
		Projectile projectile = Instantiate(bullet, spawn.position, Quaternion.identity);
		Vector3 shootDirection = transform.up;

		projectile.source = transform.root;
		projectile.direction = shootDirection;
		projectile.speed = projectileSpeed;
		projectile.lifetime = projectileLifetime;
	}
}
