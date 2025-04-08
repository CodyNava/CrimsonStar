using System.Collections.Generic;
using UnityEngine;

public class Snap : MonoBehaviour
{
    public List<Transform> _snapPoints;
    public List<Drag> _dragscripts;
    public float _snapRadius = 0.2f;

    void Update()
    {
        foreach (Drag script in _dragscripts)
        {
            script.dragEndedDelegate = SnapObject;
        }
    }

    public void SnapObject(Transform obj)
    {
        foreach (Transform point in _snapPoints)
        {
            if (Vector2.Distance(point.position, obj.position) <= _snapRadius)
            {
                obj.position = point.position;
                return;
            }
        }
    }
}