using _01_Scripts.Ship.ModuleControllers;
using System.Collections;
using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public BaseModuleController[] partPrefab;
    public GameObject partParent;
    public Snap snap;
    public ShipEditorCurrencySystem currencySystem;
    public GameObject noMoneyPopUp;
    public BridgeController bridgeController;

    public void SpawnPart(int index)
    {
        bool success = TrySpawnPart(index);
        //Todo: Handle failure here
        if (!success)
        {
            StartCoroutine(NotEnoughMoneyPopUp());
        }
    }
    IEnumerator NotEnoughMoneyPopUp()
    {
        noMoneyPopUp.SetActive(true);
        yield return new WaitForSeconds(1f);
        noMoneyPopUp.SetActive(false);
    }

    public bool TrySpawnPart(int index)
    {
        if (partPrefab == null || index < 0 || index >= partPrefab.Length) return false;

        int cost = partPrefab[index].ModuleObject._cost;
        if (currencySystem.GetCurrency() < cost)
        {
            return false;
        }


        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        BaseModuleController newPart = Instantiate(partPrefab[index], transform.position, transform.rotation);
        newPart.transform.SetParent(partParent.transform);
        newPart.gameObject.layer = LayerMask.NameToLayer("Player");
        newPart.Init(bridgeController);
        
        Drag drag = newPart.GetComponent<Drag>();
        drag.refundAction = () => { currencySystem.AddCurrency(cost); };
        snap._dragscripts.Add(drag);

        drag.ForceHold();

        currencySystem.PayCurrency(cost);
        return true;
    }
}