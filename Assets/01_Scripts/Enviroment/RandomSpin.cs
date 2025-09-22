using UnityEngine;

public class RandomSpin : MonoBehaviour
{
    [SerializeField] private int minimumMagnitude = 4;
    [SerializeField] private int maximumMagnitude = 11;

    private float randomTurn;
    private string determindRotation;
    
    void Start()
    {  
        System.Random random = new System.Random();

        // Determin which Axis to rotated and magnitude
        determindRotation = random.Next(2) == 0 ? "X-Axis" : "Y-Axis";
        randomTurn = random.Next(minimumMagnitude, maximumMagnitude);
    }

    void Update()
    {
        if (determindRotation == "X-Axis")
        {
            // Rotate around X-Axis
            transform.Rotate(randomTurn * Time.deltaTime, 0f, 0f, Space.Self);
        }
        else
        {
            // Rotate around Y-Axis
            transform.Rotate(0f, randomTurn * Time.deltaTime, 0f, Space.Self);
        }      
    }
}
