using System;
using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Projectiles;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed = 10f;
    [HideInInspector] public float lifetime = 3f;
    [HideInInspector] public Vector3 direction = Vector3.up;
    [HideInInspector] public Transform source;

    public BaseProjectileObject _BaseProjectileObject;

    private void Awake()
    {
        CombatLose_GameState.onEnterState += OnCombatOverGameState;
        CombatWin_GameState.onEnterState += OnCombatOverGameState;
    }

    private void OnCombatOverGameState(GameStateController obj)
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        CombatLose_GameState.onEnterState -= OnCombatOverGameState;
        CombatWin_GameState.onEnterState -= OnCombatOverGameState;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction.normalized * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
