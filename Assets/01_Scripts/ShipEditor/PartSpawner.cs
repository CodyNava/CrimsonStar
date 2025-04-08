using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public GameObject[] partPrefab;
    public GameObject partParent;
    public Transform SpawnPosition;
    public Snap snap;
    public ShipEditorCurrencySystem currencySystem;

    public void SpawnPart(int index)
    {
        if (partPrefab != null && index >= 0 && index < partPrefab.Length)
        {
            GameObject newPart = Instantiate(partPrefab[index], SpawnPosition.transform.position, transform.rotation);

            newPart.transform.SetParent(partParent.transform);
            snap._dragscripts.Add(newPart.GetComponent<Drag>());
            //currencySystem.currency -= partPrefab[index].GetComponent<Part>().cost;
        }
    }

}
