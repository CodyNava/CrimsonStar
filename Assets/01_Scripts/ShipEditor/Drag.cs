using System;
using UnityEngine;

[ExecuteInEditMode]
public class Drag : MonoBehaviour
{
    public delegate void DragEndedDelegate(Transform transform);

    public DragEndedDelegate dragEndedDelegate;

    private Camera _camera;
    private Vector2 _pos;
    private bool _holding;

    public Action refundAction;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (_holding)
        {
            _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = _pos;
            if (_holding && Input.GetKeyDown(KeyCode.Mouse1))
            {
                refundAction();
                Destroy(gameObject);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateParts(60);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateParts(-60);
            }
        }
    }

    public void RotateParts(int x)
    {
        gameObject.transform.Rotate(0, 0, x);
    }

    void OnMouseDown()
    {
        _holding = true;
    }

    void OnMouseUp()
    {
        _holding = false;
        dragEndedDelegate(this.transform);
    }

    public void ForceHold()
    {
        _holding = true;
    }
}