using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using _01_Scripts.Ship.ModuleControllers;
using TMPro;
using UnityEngine;

public class ShipEditor : MonoBehaviour
{
    public GameObject ship;
    public GameObject shipEditor;
    public Camera editCamera;
    public TextMeshProUGUI shipHealth;
    public TextMeshProUGUI speed;
    public Shooting[] shooting;
    public TextMeshProUGUI weapons;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShipEditor();
        }
        UIShipStats();
    }
    private void Start()
    {
        weapons.text = $"DPS: 0";
    }

    public void OpenShipEditor()
    {
        editCamera.orthographic = true;
        ship.transform.rotation = Quaternion.Euler(0, 0, 0);
        shipEditor.SetActive(true);
        GameStateController.Instance.ChangeState(new ShipEditor_GameState());
    }

    public void CloseShipEditor()
    {
        editCamera.orthographic = false;
        shipEditor.SetActive(false);
        GameStateController.Instance.ChangeState(new Combat_GameState());
    }

    public void UIShipStats()
    {
        BridgeController controler = ship.GetComponentInChildren<BridgeController>();
        shipHealth.text = $"HP: {controler.MaxHp}";
        speed.text = $"Speed: {controler.MoveSpeedChange:0.00}";
        shooting = ship.GetComponentsInChildren<Shooting>();
        if (shooting.Length == 0)
        {
            weapons.text = $"DPS: 0";
        }
        else
        {
            weapons.text = $"DPS: {shooting.Length * 10 * 2 * 2}";
        }


    }

}
