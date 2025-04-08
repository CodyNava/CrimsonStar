using UnityEngine;

public class ShipEditor : MonoBehaviour
{
    public GameObject ship;
    public GameObject shipEditor;
    public Camera editCamera;
    public float CameraOffSet;
    public float CameraOffSetOriginal;

    public void OpenShipEditor()
    {
        editCamera.fieldOfView = CameraOffSet;
        ship.SetActive(false);

        shipEditor.SetActive(true);
    }

    public void CloseShipEditor()
    {

        editCamera.fieldOfView = CameraOffSetOriginal;
        ship.SetActive(true);

        shipEditor.SetActive(false);
    }

}
