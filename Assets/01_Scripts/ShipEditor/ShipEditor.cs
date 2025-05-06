using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShipEditor : MonoBehaviour
{
    public Camera editCamera;
    //public TextMeshProUGUI shipHealth;
    //public TextMeshProUGUI speed;
    //public TextMeshProUGUI weapons;
    private int _turretCount;
    public PlayerShipEditor PlayerShipEditor { get; private set; }
    [SerializeField] private GameObject noMoneyPopUp;
    [SerializeField] private HexTransform hexTransform;
    private NetEditorModule _heldNetEditorModule;
    private int _moduleRotation;

    private Dictionary<HexCoordinate, NetEditorModule> _editorModules = new();
    public void SetPlayerShipEditor(PlayerShipEditor playerShipEditor)
    {
        PlayerShipEditor = playerShipEditor;
    }

    private void Update()
    {
        //UIShipStats();
        ModuleHolding();
    }

    private void Start()
    {
        //weapons.text = $"DPS: 0";
        editCamera ??= Camera.main;
    }

    /*public void UIShipStats()
    {
        shipHealth.text = $"HP: {_shipController.MaxHp}";
        speed.text = $"Speed: {_shipController.MoveSpeedChange:0.00}";
        shooting = ship.GetComponentsInChildren<Shooting>();
        if (shooting.Length == 0)
        {
            weapons.text = $"DPS: 0";
        }
        else
        {
            weapons.text = $"DPS: {shooting.Length * 10 * 2 * 2}";
        }


    }*/

    public void SpawnPart(NetModuleID moduleID)
    {
        bool success = TrySpawnPart(moduleID);
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


    public bool TrySpawnPart(NetModuleID moduleID)
    {
        if (_heldNetEditorModule != null)
        {
            return false;
        }

        NetModuleData moduleData = DataProvider.Instance.ModuleDB.ModuleData[moduleID];

        if (!PlayerShipEditor.ResourceStorage.HasResourcesForModule(moduleID))
        {
            return false;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        _heldNetEditorModule = Instantiate(moduleData.ShipEditorPrefab, transform.position, transform.rotation);
        PlayerShipEditor.ResourceStorage.PayForModule(moduleID);
        return true;
    }

    void ModuleHolding()
    {
        Vector2 mousePosWorld = editCamera.ScreenToWorldPoint(Input.mousePosition).xy();
        HexCoordinate cursorHexCoord = hexTransform.Layout.PositionXYToHex(mousePosWorld);
        if (_heldNetEditorModule != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (CanPlaceModule(cursorHexCoord))
                {
                    PlaceModule(cursorHexCoord);
                    return;
                }
                else
                {
                    // inplement feedback if cant place
                }
            }
            _heldNetEditorModule.transform.position = mousePosWorld.xy0();
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                PlayerShipEditor.ResourceStorage.RefundModule(_heldNetEditorModule.ModuleID);
                Destroy(_heldNetEditorModule.gameObject);
                _heldNetEditorModule = null;
            }
            if (Keyboard.current.eKey.wasPressedThisFrame && DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                RotateClockWise();
            }
            if (Keyboard.current.qKey.wasPressedThisFrame && DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                RotateCounterClockWise();
            }
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && _editorModules.TryGetValue(cursorHexCoord, out NetEditorModule placedModule))
            {
                _heldNetEditorModule = placedModule;
                RemoveModule(placedModule);
            }

        }
    }
    public bool CanPlaceModule(HexCoordinate rootCoord)
    {
        bool isAttached = false;


        foreach (HexCoordinate localCoord in _heldNetEditorModule.LocalCoordinates)
        {
            HexCoordinate coord = rootCoord + localCoord;
            _editorModules[coord] = _heldNetEditorModule;

            if (PlayerShipEditor.ModuleStorage.IsCoordinateOccupied(coord))
            {
                return false;
            }
            if (PlayerShipEditor.ModuleStorage.IsNeighboringModule(coord))
            {
                isAttached = true;
            }
        }
        return isAttached;
    }
    public void PlaceModule(HexCoordinate rootCoord)
    {
        _heldNetEditorModule.PlacedLocation = rootCoord;
        foreach (HexCoordinate localCoord in _heldNetEditorModule.LocalCoordinates)
        {
            HexCoordinate coord = rootCoord + localCoord;
            _editorModules[coord] = _heldNetEditorModule;
            PlayerShipEditor.ModuleStorage.AddModule(rootCoord, _heldNetEditorModule.ModuleID);
        }
        _heldNetEditorModule.transform.position = hexTransform.Layout.HexToPositionXY(rootCoord).xy0();
        _heldNetEditorModule = null;
    }
    public void RemoveModule(NetEditorModule moduleToRemove)
    {
        foreach (HexCoordinate localCoord in moduleToRemove.LocalCoordinates)
        {
            HexCoordinate coord = moduleToRemove.PlacedLocation + localCoord;
            _editorModules.Remove(coord);
            PlayerShipEditor.ModuleStorage.RemoveModule(moduleToRemove.PlacedLocation);
        }
    }
    public void RotateClockWise()
    {
        _moduleRotation++;
        if (_moduleRotation > 5)
        {
            _moduleRotation -= 6;
        }
        SetTransformRotation();
        _heldNetEditorModule.RotateClockwise();
    }
    public void RotateCounterClockWise()
    {
        _moduleRotation--;
        if (_moduleRotation < 0)
        {
            _moduleRotation += 6;
        }
        SetTransformRotation();
        _heldNetEditorModule.RotateCounterclockwise();
    }
    private void SetTransformRotation()
    {
        _heldNetEditorModule.transform.rotation = Quaternion.AngleAxis(_moduleRotation * 60, Vector3.back);
    }
}
