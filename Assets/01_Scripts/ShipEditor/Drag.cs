using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class Drag : MonoBehaviour
{
    public delegate void DragEndedDelegate(Transform transform);

    public DragEndedDelegate dragEndedDelegate;

    private Camera _camera;
    private Vector2 _pos;
    private bool _holding;
    public PolygonCollider2D _collider;

    public Action refundAction;

    private void Awake()
    {
        Combat_GameState.onEnterState -= OnCombatEnter;
        Combat_GameState.onExitState -= OnCombatExit;
        Combat_GameState.onEnterState += OnCombatEnter;
        Combat_GameState.onExitState += OnCombatExit;
    }
    private void OnDestroy()
    {
        Combat_GameState.onEnterState -= OnCombatEnter;
        Combat_GameState.onExitState -= OnCombatExit;
    }

    private void OnCombatExit()
    {
        enabled = true;
    }

    private void OnCombatEnter(GameStateController controller)
    {
        enabled = false;
    }

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (_holding)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _holding = false;
                dragEndedDelegate(this.transform);
                return;
            }
            _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = _pos;
            if (_holding && Input.GetKeyDown(KeyCode.Mouse1))
            {
                refundAction();
                Destroy(gameObject);
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                RotateParts(60);
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                RotateParts(-60);
            }
        }
        else
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Mouse.current.leftButton.wasPressedThisFrame && Physics.Raycast(ray, out var hit) && hit.collider.transform.parent == transform)
            {
                _holding = true;
            }
        }
    }
    public void RotateParts(int x)
    {
        gameObject.transform.Rotate(0, 0, x);
    }

    public void ForceHold()
    {
        _holding = true;
    }
}