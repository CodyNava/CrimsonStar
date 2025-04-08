using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public GameObject partPrefab;
    public GameObject partParent;
    public Transform SpawnPosition;

    public void SpawnPart()
    {
        if (partPrefab != null)
        {
            GameObject newPart = Instantiate(partPrefab, transform.position = new Vector3(0, 0, 0), transform.rotation = Quaternion.Euler(0, 0, 30));
            newPart.transform.SetParent(partParent.transform);
        }
        else
        {
            Debug.LogError("Part prefab is not assigned in the inspector.");
        }
    }
}
