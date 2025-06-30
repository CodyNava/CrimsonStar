using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Button = UnityEngine.UI.Button;

public class ShipEditor : MonoBehaviour
{
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int _OutlineThickness = Shader.PropertyToID("_OutLineThickNess");
    private static readonly int Thickness = Shader.PropertyToID("_OutlineThickness");
    [SerializeField] private Camera editCamera;
    [SerializeField] private GameObject noMoneyPopUp;
    [SerializeField] private GameObject cantBePlacedPopUp;
    [SerializeField] private TextMeshProUGUI cantBePlacedText;
    [SerializeField] private HexTransform hexTransform;
    [SerializeField] private ShipEditorStats shipEditorStats;
    [SerializeField] private NetEditorModule netEditorBridgeRef;
    [SerializeField] private Button energyViewToggleButton;
    [SerializeField] private Material outlineShader;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private float moduleMoveSpeedGP;
    [SerializeField] public bool inEnergyView;
    [SerializeField] public bool inEnergyViewParentToggle;

    [SerializeField] public bool moduleSpawnedInGP;
    [SerializeField] public bool reactorIsNearby;

    private Color originalOutlineShaderColor;
    private float originalOutlineShaderStrenght;

    [SerializeField] private FMODUnity.EventReference modulePlacedEvent;
    public NetMatchPlayer PlayerData { get; private set; }

    private Dictionary<HexCoordinate, NetEditorModule> _editorModulesMap = new();

    [SerializedDictionary("EnergyMap")] public SerializedDictionary<HexCoordinate, int> energyMap = new();
    private NetEditorModule _heldNetEditorModule;

    private List<NetEditorModule> _editorModuleList;

    private void Update()
    {
        if (InputManager.Instance.IsGamepadUsed)
        {
            if (moduleSpawnedInGP)
            {
                moduleSpawnedInGP = false;
                return;
            }

            ModueHoldingGamePad();
        }
        else
        {
            ModuleHoldingKeyboard();
        }
    }

    private void LateUpdate()
    {
        IsPowereableInRangeOfReactor();
    }

    private void Start()
    {
        editCamera ??= Camera.main;
        ModuleSelectionButton.ModuleSelected -= SpawnPart;
        ModuleSelectionButton.ModuleSelected += SpawnPart;
        _editorModuleList = new List<NetEditorModule> // collection initialization syntax uwu
        {
            netEditorBridgeRef
        };
        StartCoroutine(LinkPlayerRoutine());
        shipEditorStats.GetTotalStats(_editorModuleList);
        // energyViewToggleButton.onClick.AddListener(ToggleEnergyView);
        originalOutlineShaderColor = outlineShader.GetColor(OutlineColor);
        originalOutlineShaderStrenght = outlineShader.GetFloat("_OutlineThickness");
    }

    private IEnumerator LinkPlayerRoutine()
    {
        while (PlayerData == null)
        {
            var players = FindObjectsByType<NetMatchPlayer>(FindObjectsSortMode.None);
            foreach (var matchPlayer in players)
            {
                if (matchPlayer.IsOwner)
                {
                    PlayerData = matchPlayer;
                    ReconstructShip(PlayerData.ModuleStorage.GetUniqueModules());
                    yield break;
                }
            }

            yield return null;
        }
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

    private IEnumerator CantBePlacedPopUp(bool cantPlaceReactor)
    {
        StartCoroutine(OutlineShaderChanger());
        if (cantPlaceReactor)
        {
            cantBePlacedPopUp.SetActive(true);
            cantBePlacedText.text =
                $"Cant be Placed\nAnother Reactor in Range";
            yield return new WaitForSeconds(3f);
            cantBePlacedPopUp.SetActive(false);
        }
    }

    private IEnumerator OutlineShaderChanger()
    {
        outlineShader.SetColor(OutlineColor, Color.red);
        outlineShader.SetFloat("_OutlineThickness", originalOutlineShaderStrenght * 2.5f);
        yield return new WaitForSeconds(0.4f);
        outlineShader.SetColor(OutlineColor, originalOutlineShaderColor);
        outlineShader.SetFloat("_OutlineThickness", originalOutlineShaderStrenght);
    }

    public bool TrySpawnPart(NetModuleID moduleID)
    {
        if (_heldNetEditorModule != null)
        {
            return false;
        }

        if (!PlayerData.C_CanAffordModule(moduleID))
        {
            return false;
        }

        lastSelected = EventSystem.current.currentSelectedGameObject;
        if (InputManager.Instance.IsGamepadUsed)
        {
            moduleSpawnedInGP = true;
            Vector3 spawnVec = new Vector3(0f, 0f, 0f);
            _heldNetEditorModule = Instantiate(moduleID.GetModuleData().ShipEditorPrefab, spawnVec, transform.rotation);
        }
        else
        {
            _heldNetEditorModule = Instantiate(moduleID.GetModuleData().ShipEditorPrefab,
                editCamera.ScreenToWorldPoint(Input.mousePosition).xy(),
                transform.rotation);
        }

        _heldNetEditorModule.Initialize();
        PlayerData.C_PayForModule(moduleID);
        _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Outline");
        return true;
    }

    private void ModueHoldingGamePad() // Important for radial menu
    {
        var t = Time.deltaTime;
        var v = moduleMoveSpeedGP;
        moveInput = Keybinds.Actions.ShipEditor.MoveModule.ReadValue<Vector2>();
        moveInput.Normalize();
        Debug.Log(moveInput);
        HexCoordinate cursorHexCoord = hexTransform.Layout.PositionXYToHex(_heldNetEditorModule.transform.position);

        if (_heldNetEditorModule != null)
        {
            HandleHeldEnergyModule(cursorHexCoord);
            ToggleOnEnergyViewBasedOnMudule();
            EventSystem.current.SetSelectedGameObject(null);
            _heldNetEditorModule.transform.Translate(moveInput.x * v * t, moveInput.y * v * t, 0f, Space.World);
            if (Keybinds.Actions.ShipEditor.ModulePickOrDrop.WasPerformedThisFrame())
            {
                if (CanPlaceModule(cursorHexCoord))
                {
                    PlayerData.ModuleStorage.C_AddModule(cursorHexCoord, _heldNetEditorModule.ModuleID,
                        _heldNetEditorModule.PlacedRotation);
                    NetModuleID id = _heldNetEditorModule.ModuleID;
                    PlaceModule(cursorHexCoord);
                    FMODUnity.RuntimeManager.PlayOneShot(modulePlacedEvent, transform.position);
                    if (Keyboard.current.leftShiftKey.isPressed)
                    {
                        SpawnPart(id);
                    }

                    return;
                }
                else
                {
                    StartCoroutine(CantBePlacedPopUp(IsReactorInRangeOfReactor(cursorHexCoord)));
                }
            }

            //_heldNetEditorModule.transform.position = moveInput.xy0();
            if (Keybinds.Actions.ShipEditor.ModuleSell.WasPerformedThisFrame())
            {
                PlayerData.C_RefundModule(_heldNetEditorModule.ModuleID);
                Destroy(_heldNetEditorModule.gameObject);
                _heldNetEditorModule = null;
                EventSystem.current.SetSelectedGameObject(lastSelected);
                return;
            }

            if (Keybinds.Actions.ShipEditor.RotateModuleRight.WasPerformedThisFrame() &&
                DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                _heldNetEditorModule.C_RotateClockwise();
            }

            if (Keybinds.Actions.ShipEditor.RotateModuleLeft.WasPerformedThisFrame() &&
                DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                _heldNetEditorModule.C_RotateCounterclockwise();
            }
        }
    }
    private void ModuleHoldingKeyboard()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector2 mousePosWorld = editCamera.ScreenToWorldPoint(Input.mousePosition).xy();
        HexCoordinate cursorHexCoord = hexTransform.Layout.PositionXYToHex(mousePosWorld);
        if (_heldNetEditorModule != null)
        {
            HandleHeldEnergyModule(cursorHexCoord);
            ToggleOnEnergyViewBasedOnMudule();
            if (Keybinds.Actions.ShipEditor.ModulePickOrDrop.WasReleasedThisFrame())
            {
                if (CanPlaceModule(cursorHexCoord))
                {
                    PlayerData.ModuleStorage.C_AddModule(cursorHexCoord, _heldNetEditorModule.ModuleID,
                        _heldNetEditorModule.PlacedRotation);
                    NetModuleID id = _heldNetEditorModule.ModuleID;
                    PlaceModule(cursorHexCoord);
                    FMODUnity.RuntimeManager.PlayOneShot(modulePlacedEvent, transform.position);
                    if (Keyboard.current.leftShiftKey.isPressed)
                    {
                        SpawnPart(id);
                    }

                    return;
                }
                else
                {
                    StartCoroutine(CantBePlacedPopUp(IsReactorInRangeOfReactor(cursorHexCoord)));
                }
            }

            _heldNetEditorModule.transform.position = mousePosWorld.xy0();
            if (Keybinds.Actions.ShipEditor.ModuleSell.WasPerformedThisFrame())
            {
                PlayerData.C_RefundModule(_heldNetEditorModule.ModuleID);
                Destroy(_heldNetEditorModule.gameObject);
                _heldNetEditorModule = null;
                return;
            }

            if (Keybinds.Actions.ShipEditor.RotateModuleRight.WasPerformedThisFrame() &&
                DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                _heldNetEditorModule.C_RotateClockwise();
            }

            if (Keybinds.Actions.ShipEditor.RotateModuleLeft.WasPerformedThisFrame() &&
                DataProvider.Instance.ModuleDB.ModuleData[_heldNetEditorModule.ModuleID].CanRotate)
            {
                _heldNetEditorModule.C_RotateCounterclockwise();
            }
        }
        else
        {
            if (Keybinds.Actions.ShipEditor.ModulePickOrDrop.WasPerformedThisFrame() &&
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
        if (IsReactorInRangeOfReactor(rootCoord))
        {
            Debug.Log("ReactorInRange");
            return false;
        }

        foreach (HexCoordinate localCoord in _heldNetEditorModule.LocalCoordinates)
        {
            HexCoordinate coord = rootCoord + localCoord;
            var moduleID = _heldNetEditorModule.ModuleID;
            var condition = GetPlacementConditionForModule(moduleID);
            if (PlayerData.ModuleStorage.SC_IsCoordinateOccupied(coord) || condition(rootCoord) ||
                IsThrusterAbove(coord))
            {
                return false;
            }

            if (PlayerData.ModuleStorage.SC_IsNeighboringModule(coord))
            {
                isAttached = true;
            }
        }

        return isAttached;
    }

    private Func<HexCoordinate, bool> GetPlacementConditionForModule(NetModuleID moduleID)
    {
        if (moduleID == NetModuleID.Thruster)
        {
            return IsSomethingBelowThruster;
        }

        return IsThrusterAbove;
    }

    private bool IsThrusterAbove(HexCoordinate coord)
    {
        const int maxDistance = 3;
        for (var i = 0; i < maxDistance; i++)
        {
            coord = HexCoordinate.Neighbor(coord, HexDirection.North);
            if (_editorModulesMap.TryGetValue(coord, out var module))
            {
                if (module.ModuleID == NetModuleID.Thruster)
                {
                    Debug.Log("is Above" + module);
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsSomethingBelowThruster(HexCoordinate coord)
    {
        const int maxDistance = 3;
        for (var i = 0; i < maxDistance; i++)
        {
            coord = HexCoordinate.Neighbor(coord, HexDirection.South);
            if (_editorModulesMap.TryGetValue(coord, out var value))
            {
                Debug.Log("is below" + value);
                return true;
            }
        }

        return false;
    }

    private void AddPowerToEnergyMap(HexCoordinate coord, bool reactorPlaced, int range)
    {
        foreach (HexCoordinate neighborCoord in coord.CoordinatesInRange(range))
        {
            int power = energyMap.GetValueOrDefault(neighborCoord);
            if (neighborCoord == coord) continue;
            energyMap[neighborCoord] = reactorPlaced ? power + 1 : power - 1;
        }
    }

    public bool CheckIfPowered(HexCoordinate coord) => energyMap.GetValueOrDefault(coord) >= 1;

    private bool IsReactorInRangeOfReactor(HexCoordinate coord)
    {
        if (_heldNetEditorModule.ModuleID == NetModuleID.Reactor)
        {
            foreach (HexCoordinate neighborCoord in coord.CoordinatesInRange(2))
            {
                if (_editorModulesMap.TryGetValue(neighborCoord, out var module))
                {
                    if (module.ModuleID == NetModuleID.Reactor)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void IsPowereableInRangeOfReactor()
    {
        if (_heldNetEditorModule != null && _heldNetEditorModule.ModuleID == NetModuleID.Reactor)
        {
            Vector2 mousePosWorld = editCamera.ScreenToWorldPoint(Input.mousePosition).xy();
            HexCoordinate cursorHexCoord = hexTransform.Layout.PositionXYToHex(mousePosWorld);

            foreach (HexCoordinate neighborCoord in cursorHexCoord.CoordinatesInRange(2))
            {
                if (_editorModulesMap.TryGetValue(neighborCoord, out var module))
                {
                    if (module.ModuleData.CanBePowered)
                    {
                        module.IsPowered = true;
                    }
                }
            }
        }
    }


    private bool IsConnectionToBridge(HexCoordinate coord)
    {
        return false;
    }

    public void ToggleEnergyView()
    {
        inEnergyView = !inEnergyView;
        inEnergyViewParentToggle = !inEnergyViewParentToggle;
    }

    private void ToggleOnEnergyViewBasedOnMudule()
    {
        if (!inEnergyView && !inEnergyViewParentToggle && _heldNetEditorModule.ModuleID == NetModuleID.Reactor
            || _heldNetEditorModule.ModuleData.CanBePowered)
        {
            inEnergyView = true;
        }
    }

    private void ToggleOffEnergyViewBasedOnMudule()
    {
        if (inEnergyView && _heldNetEditorModule.ModuleID == NetModuleID.Reactor
            || _heldNetEditorModule.ModuleData.CanBePowered)
        {
            inEnergyView = inEnergyViewParentToggle;
        }
    }

    public void HandleHeldEnergyModule(HexCoordinate coord)
    {
        if (_heldNetEditorModule.ModuleData.CanBePowered)
            _heldNetEditorModule.PlacedLocation = coord;
    }

    public void PlaceModule(HexCoordinate rootCoord)
    {
        if (_heldNetEditorModule.ModuleID == NetModuleID.Reactor)
        {
            AddPowerToEnergyMap(rootCoord, true, _heldNetEditorModule.ModuleData.EffectRange);
        }

        ToggleOffEnergyViewBasedOnMudule();
        _heldNetEditorModule.PlacedLocation = rootCoord;
        foreach (HexCoordinate localCoord in _heldNetEditorModule.LocalCoordinates)
        {
            HexCoordinate coord = rootCoord + localCoord;
            _editorModulesMap[coord] = _heldNetEditorModule;
        }

        _heldNetEditorModule.transform.position = hexTransform.Layout.HexToPositionXY(rootCoord).xy0();
        _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Modules");
        _editorModuleList.Add(_heldNetEditorModule);
        shipEditorStats.GetTotalStats(_editorModuleList);
        _heldNetEditorModule = null;
        EventSystem.current.SetSelectedGameObject(lastSelected);
    }

    public void RemoveModule(NetEditorModule moduleToRemove)
    {
        if (InputManager.Instance.IsGamepadUsed)
        {
            _heldNetEditorModule = moduleToRemove;
        }

        if (_heldNetEditorModule.ModuleID == NetModuleID.Reactor)
            AddPowerToEnergyMap(moduleToRemove.PlacedLocation, false, _heldNetEditorModule.ModuleData.EffectRange);
        foreach (HexCoordinate localCoord in moduleToRemove.LocalCoordinates)
        {
            HexCoordinate coord = moduleToRemove.PlacedLocation + localCoord;
            _editorModulesMap.Remove(coord);
        }


        PlayerData.ModuleStorage.C_RemoveModule(moduleToRemove.PlacedLocation);
        _editorModuleList.Remove(_heldNetEditorModule);
        shipEditorStats.GetTotalStats(_editorModuleList);
    }

    public void SignalReady()
    {
        if (PlayerData.C_SignalReady())
        {
            gameObject.SetActive(false);
        }
    }

    public void ReconstructShip(IEnumerable<ModulePlacementData> uniqueModules)
    {
        foreach (ModulePlacementData uniqueModule in uniqueModules)
        {
            _heldNetEditorModule = Instantiate(uniqueModule.ModuleID.GetModuleData().ShipEditorPrefab,
                new InstantiateParameters { scene = gameObject.scene });
            _heldNetEditorModule.Initialize();

            for (int i = 0; i < uniqueModule.Rotation; i++)
            {
                _heldNetEditorModule.C_RotateClockwise();
            }

            PlaceModule(uniqueModule.RootCoordinate);
        }
    }
}