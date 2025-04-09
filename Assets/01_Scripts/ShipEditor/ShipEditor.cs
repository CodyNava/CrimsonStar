using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using UnityEngine;

public class ShipEditor : MonoBehaviour
{
    public GameObject ship;
    public GameObject shipEditor;
    public Camera editCamera;

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

}
