using UnityEngine;

public class Spin : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0f, 1 * Time.deltaTime, 0f, Space.Self);
    }
}
