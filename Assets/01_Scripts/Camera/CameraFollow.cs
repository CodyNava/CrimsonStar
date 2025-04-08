using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject ship;
    public Vector3 Offset;
    public void Update()
    {
        FollowObject();
    }
    public void FollowObject()
    {
        if (ship)
        {
            gameObject.transform.position = ship.transform.position + Offset;

        }
    }
}
