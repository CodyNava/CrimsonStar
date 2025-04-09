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