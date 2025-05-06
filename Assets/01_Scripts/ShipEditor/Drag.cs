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
    public GameObject child;
    [SerializeField] private NetEditorModule netEditorModule;
    public Action refundAction;
    private int _moduleRotation;
    void Start()
    {
        _camera = Camera.main;
        child.layer = LayerMask.NameToLayer("Modules");
    }

    void Update()
    {
        if (_holding)
        {
            child.layer = LayerMask.NameToLayer("Outline");
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _holding = false;
                dragEndedDelegate(this.transform);
                child.layer = LayerMask.NameToLayer("Modules");
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
                RotateClockWise();
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                RotateCounterClockWise();
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
    public void RotateClockWise()
    {
        _moduleRotation++;
        if (_moduleRotation > 5)
        {
            _moduleRotation -= 6;
        }
        SetTransformRotation();
        netEditorModule.RotateClockwise();
    }
    public void RotateCounterClockWise()
    {
        _moduleRotation--;
        if (_moduleRotation < 0)
        {
            _moduleRotation += 6;
        }
        SetTransformRotation();
        netEditorModule.RotateCounterclockwise();
    }
    private void SetTransformRotation()
    {
        transform.rotation = Quaternion.AngleAxis(_moduleRotation * 60, Vector3.back);
    }
    public void ForceHold()
    {
        _holding = true;
    }
}