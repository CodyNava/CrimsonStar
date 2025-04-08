using UnityEngine;

[ExecuteInEditMode]
public class Drag : MonoBehaviour
{
    public delegate void DragEndedDelegate(Transform transform);

    public DragEndedDelegate dragEndedDelegate;

    private Camera _camera;
    private Vector2 _pos;
    private bool _holding;

    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (_holding)
        {
            _pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = _pos;
        }
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
}