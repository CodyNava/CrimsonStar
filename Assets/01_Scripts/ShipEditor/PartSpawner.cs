using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public GameObject[] partPrefab;
    public GameObject partParent;
    public Transform SpawnPosition;
    public Snap snap;

    public void SpawnPart(int index)
    {
        if (partPrefab != null && index >= 0 && index < partPrefab.Length)
        {
            GameObject newPart = Instantiate(
                partPrefab[index],
                SpawnPosition.position,
                Quaternion.Euler(0, 0, 30)
            );

            newPart.transform.SetParent(partParent.transform);
            snap._dragscripts.Add(newPart.GetComponent<Drag>());
        }
    }

}
