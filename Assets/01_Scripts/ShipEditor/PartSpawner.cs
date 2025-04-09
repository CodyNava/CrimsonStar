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
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            GameObject newPart = Instantiate(partPrefab[index], transform.position, transform.rotation);
            newPart.transform.SetParent(partParent.transform);

            Drag drag = newPart.GetComponent<Drag>();
            snap._dragscripts.Add(drag);

            drag.ForceHold();

        }
    }

}
