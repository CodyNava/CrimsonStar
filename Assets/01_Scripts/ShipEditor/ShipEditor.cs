using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipEditor : MonoBehaviour
{
    [SerializeField] private Camera editCamera;
    [SerializeField] private GameObject noMoneyPopUp;
    [SerializeField] private GameObject cantBePlacedPopUp;
    [SerializeField] private HexTransform hexTransform;
    [SerializeField] private ShipEditorStats shipEditorStats;
    [SerializeField] private NetEditorModule netEditorBridgeRef;
    public NetShipEditorData NetShipEditorData { get; private set; }

    private Dictionary<HexCoordinate, NetEditorModule> _editorModulesMap = new();
    private NetEditorModule _heldNetEditorModule;

    private List<NetEditorModule> editorModuleList;

    public void SetPlayerShipEditor(NetShipEditorData netShipEditorData)
    {
        NetShipEditorData = netShipEditorData;
    }

    private void Update()
    {
        ModuleHolding();
    }

    private void Start()
    {
        editCamera ??= Camera.main;
        ModuleSelectionButton.ModuleSelected -= SpawnPart;
        ModuleSelectionButton.ModuleSelected += SpawnPart;
        editorModuleList = new List<NetEditorModule> // collection initialization syntax uwu
        {
            netEditorBridgeRef
        };
        shipEditorStats.GetTotalStats(editorModuleList);
    }

    private void OnDestroy()
    {
        ModuleSelectionButton.ModuleSelected -= SpawnPart;
    }

    public void SpawnPart(NetModuleID moduleID)
    {
        bool success = TrySpawnPart(moduleID);
        if (!success)
        {
            StartCoroutine(NotEnoughMoneyPopUp());
        }
    }

    private IEnumerator NotEnoughMoneyPopUp()
    {
        noMoneyPopUp.SetActive(true);
        yield return new WaitForSeconds(1f);
        noMoneyPopUp.SetActive(false);
    }

    private IEnumerator CantBePlacedPopUp()
    {
        cantBePlacedPopUp.SetActive(true);
        yield return new WaitForSeconds(1f);
        cantBePlacedPopUp.SetActive(false);
    }

    public bool TrySpawnPart(NetModuleID moduleID)
    {
        if (_heldNetEditorModule != null)
        {
            return false;
        }

        if (!NetShipEditorData.ResourceStorage.SC_HasResourcesForModule(moduleID))
        {
            return false;
        }

        _heldNetEditorModule =
            Instantiate(moduleID.GetModuleData().ShipEditorPrefab, transform.position, transform.rotation);
        _heldNetEditorModule.Initialize();
        NetShipEditorData.ResourceStorage.C_PayForModule(moduleID);
        _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Outline");
        return true;
    }

    private void ModuleHolding()
    {
        Vector2 mousePosWorld = editCamera.ScreenToWorldPoint(Input.mousePosition).xy();
        HexCoordinate cursorHexCoord = hexTransform.Layout.PositionXYToHex(mousePosWorld);
        if (_heldNetEditorModule != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (CanPlaceModule(cursorHexCoord))
                {
                    NetShipEditorData.ModuleStorage.C_AddModule(cursorHexCoord, _heldNetEditorModule.ModuleID,
                        _heldNetEditorModule.PlacedRotation);
                    PlaceModule(cursorHexCoord);
                    return;
                }
                else
                {
                    cantBePlacedPopUp.transform.position = mousePosWorld;
                    StartCoroutine(CantBePlacedPopUp());
                }
            }

            _heldNetEditorModule.transform.position = mousePosWorld.xy0();
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                NetShipEditorData.ResourceStorage.C_RefundModule(_heldNetEditorModule.ModuleID);
                Destroy(_heldNetEditorModule.gameObject);
                _heldNetEditorModule = null;
                return;
            }

            if (Keyboard.current.eKey.wasPressedThisFrame &&
                DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                _heldNetEditorModule.C_RotateClockwise();
            }

            if (Keyboard.current.qKey.wasPressedThisFrame &&
                DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                _heldNetEditorModule.C_RotateCounterclockwise();
            }
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame &&
                _editorModulesMap.TryGetValue(cursorHexCoord, out NetEditorModule placedModule))
            {
                _heldNetEditorModule = placedModule;
                RemoveModule(placedModule);
                _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Outline");
            }
        }
    }

    public bool CanPlaceModule(HexCoordinate rootCoord)
    {
        bool isAttached = false;

        foreach (HexCoordinate localCoord in _heldNetEditorModule.LocalCoordinates)
        {
            HexCoordinate coord = rootCoord + localCoord;
            _editorModulesMap[coord] = _heldNetEditorModule;

            if (NetShipEditorData.ModuleStorage.SC_IsCoordinateOccupied(coord))
            {
                return false;
            }

            if (NetShipEditorData.ModuleStorage.SC_IsNeighboringModule(coord))
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
            _editorModulesMap[coord] = _heldNetEditorModule;
        }


        _heldNetEditorModule.transform.position = hexTransform.Layout.HexToPositionXY(rootCoord).xy0();
        _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Modules");
        editorModuleList.Add(_heldNetEditorModule);
        shipEditorStats.GetTotalStats(editorModuleList);
        _heldNetEditorModule = null;
    }

    public void RemoveModule(NetEditorModule moduleToRemove)
    {
        foreach (HexCoordinate localCoord in moduleToRemove.LocalCoordinates)
        {
            HexCoordinate coord = moduleToRemove.PlacedLocation + localCoord;
            _editorModulesMap.Remove(coord);
        }

        NetShipEditorData.ModuleStorage.C_RemoveModule(moduleToRemove.PlacedLocation);
        editorModuleList.Remove(_heldNetEditorModule);
        shipEditorStats.GetTotalStats(editorModuleList);
    }

    public void SignalReady()
    {
        if (NetShipEditorData.SignalReady())
        {
            gameObject.SetActive(false);
        }
    }

    public void ReconstructShip(IEnumerable<ModulePlacementData> uniqueModules
    )
    {
        foreach (ModulePlacementData uniqueModule in uniqueModules)
        {
            _heldNetEditorModule = Instantiate(uniqueModule.ModuleID.GetModuleData().ShipEditorPrefab);
            _heldNetEditorModule.Initialize();

            for (int i = 0; i < uniqueModule.Rotation; i++)
            {
                _heldNetEditorModule.C_RotateClockwise();
            }

            PlaceModule(uniqueModule.RootCoordinate);
        }
    }
}