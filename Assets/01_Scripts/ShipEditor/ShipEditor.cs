using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [SerializeField] private ShipEditorWeaponGroups weaponGroupManager;
    [SerializeField] private Material outlineShader;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private float moduleMoveSpeedGP;
    [SerializeField] List<GameObject> gamePadSwitches = new List<GameObject>();

    [Header("HealthView")] [SerializeField]
    public TextMeshProUGUI healthViewModeText;

    [SerializeField] private Button healthViewToggleButton;
    [SerializeField] private Sprite healthViewImageToggled;
    [SerializeField] private Sprite healthViewImageNormal;
    [SerializeField] public bool inHealthView;
    [SerializeField] public bool inPercentageHealthView;

    [Header("EnergyView")] [SerializeField]
    private Button energyViewToggleButton;

    [SerializeField] private Sprite energyViewButtonImageToggled;
    [SerializeField] private Sprite energyViewButtonImageNormal;
    [SerializeField] public bool inEnergyView;
    [SerializeField] public bool inEnergyViewParentToggle;
    [SerializeField] public bool joiningEditor;


    [SerializeField] public bool moduleSpawnedInGP;
    [SerializeField] public bool moduleFirstSelectedGP;
    [SerializeField] public bool reactorIsNearby;

    private Color originalOutlineShaderColor;
    private float originalOutlineShaderStrenght;

    [SerializeField] private FMODUnity.EventReference modulePlacedEvent;
    [SerializeField] private FMODUnity.EventReference moduleRefundEvent;
    [SerializeField] private FMODUnity.EventReference moduleBuyEvent;
    public NetMatchPlayer PlayerData { get; private set; }

    private Dictionary<HexCoordinate, NetEditorModule> _editorModulesMap = new();

    [SerializedDictionary("EnergyMap")] public SerializedDictionary<HexCoordinate, int> energyMap = new();

    private NetEditorModule _heldNetEditorModule;

    private List<NetEditorModule> _editorModuleList;
    public IReadOnlyList<NetEditorModule> EditorModuleList => _editorModuleList;

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

        ToggleGamePadSwitches();
        ToggleViewsViaKeys();
    }

    private void LateUpdate()
    {
        IsPowerableInRangeOfReactor();
    }

    public void LeaveEditor()
    {
        if (global::PlayerData.CurrentLobbyID != CSteamID.Nil)
            NetGameBootstrapper.LeaveLobby();
        else
        {
            NetGameBootstrapper.LeaveLobbyLocal();
        }
        
        SceneAudioManager.instance.StopInGameMusic();
        SceneAudioManager.instance.ResetMusicProgress();
        SceneManager.LoadScene("MainMenu");
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
        shipEditorStats.GetTotalStats(_editorModuleList, weaponGroupManager);
        // energyViewToggleButton.onClick.AddListener(ToggleEnergyView);
        originalOutlineShaderColor = outlineShader.GetColor(OutlineColor);
        originalOutlineShaderStrenght = outlineShader.GetFloat("_OutlineThickness");
        inHealthView = false;
        inPercentageHealthView = false;
        SetOverLayModulesColourViaHealthMap();
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

        FMODUnity.RuntimeManager.PlayOneShot(moduleBuyEvent, _heldNetEditorModule.transform.position);
        _heldNetEditorModule.Initialize();
        PlayerData.C_PayForModule(moduleID);
        _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Outline");
        return true;
    }

    private void ToggleGamePadSwitches()
    {
        foreach (var switches in gamePadSwitches)
        {
            switches.SetActive(InputManager.Instance.IsGamepadUsed);
        }
    }

    private void ModueHoldingGamePad() // Important for radial menu
    {
        var t = Time.deltaTime;
        var v = moduleMoveSpeedGP;
        moveInput = Keybinds.Actions.ShipEditor.MoveModule.ReadValue<Vector2>();
        moveInput.Normalize();
        Debug.Log(moveInput);

        if (_heldNetEditorModule != null)
        {
            HexCoordinate cursorHexCoord = hexTransform.Layout.PositionXYToHex(_heldNetEditorModule.transform.position);
            HandleHeldEnergyModule(cursorHexCoord);
            ToggleOnEnergyViewBasedOnModule();
            EventSystem.current.SetSelectedGameObject(null);
            _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Outline");
            _heldNetEditorModule.transform.Translate(moveInput.x * v * t, moveInput.y * v * t, 0f, Space.World);
            if (Keybinds.Actions.ShipEditor.ModulePickOrDrop.WasPerformedThisFrame())
            {
                if (moduleFirstSelectedGP)
                {
                    moduleFirstSelectedGP = false;
                    return;
                }

                if (CanPlaceModule(cursorHexCoord))
                {
                    PlayerData.ModuleStorage.C_AddModule(cursorHexCoord, _heldNetEditorModule.ModuleID,
                        _heldNetEditorModule.PlacedRotation);
                    NetModuleID id = _heldNetEditorModule.ModuleID;
                    PlaceModule(cursorHexCoord);
                    FMODUnity.RuntimeManager.PlayOneShot(modulePlacedEvent, transform.position);
                    EventSystem.current.SetSelectedGameObject(lastSelected);
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
                FMODUnity.RuntimeManager.PlayOneShot(moduleRefundEvent, _heldNetEditorModule.transform.position);
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
            ToggleOnEnergyViewBasedOnModule();
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
                FMODUnity.RuntimeManager.PlayOneShot(moduleRefundEvent, _heldNetEditorModule.transform.position);
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


    public void ChangeLayerOnEachModule()
    {
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

    public void IsPowerableInRangeOfReactor()
    {
        if (_heldNetEditorModule && _heldNetEditorModule.ModuleID == NetModuleID.Reactor)
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

    public void ToggleViewsViaKeys()
    {
        if (Keybinds.Actions.ShipEditor.EnergyOverview.WasPerformedThisFrame())
        {
            ToggleEnergyView();
        }

        if (Keybinds.Actions.ShipEditor.HealthOverview.WasPerformedThisFrame())
        {
            ToggleHealthView();
        }

        if (Keybinds.Actions.ShipEditor.Ready.WasPerformedThisFrame())
        {
            SignalReady();
        }
    }

    public void ToggleEnergyView()
    {
        inEnergyView = !inEnergyView;
        inEnergyViewParentToggle = !inEnergyViewParentToggle;
        ToggleEnergyViewButtonImage();
    }

    public void ToggleHealthView()
    {
        inHealthView = !inHealthView;
        int layer = LayerMask.NameToLayer("HealthOverLay");
        if (layer == -1) return;
        bool layerToggled = (editCamera.cullingMask & (1 << layer)) != 0;
        editCamera.cullingMask = layerToggled
            ? editCamera.cullingMask &= ~(1 << layer)
            : editCamera.cullingMask |= 1 << layer;
        ToggleHealthViewButtonImage();
        if (inHealthView) SetOverLayModulesColourViaHealthMap();
    }

    public void TogglePercentageHealthView()
    {
        inPercentageHealthView = !inPercentageHealthView;
        healthViewModeText.text = inPercentageHealthView ? "%" : "Total";
        if (inHealthView) SetOverLayModulesColourViaHealthMap();
    }

    private void ToggleHealthViewButtonImage()
    {
        var healthButtonImage = healthViewToggleButton.GetComponent<Image>();
        healthButtonImage.sprite = inHealthView ? healthViewImageToggled : healthViewImageNormal;
        if (!InputManager.Instance.IsGamepadUsed) EventSystem.current.SetSelectedGameObject(null);
    }

    private void ToggleOnEnergyViewBasedOnModule()
    {
        if (!inEnergyView && !inEnergyViewParentToggle && _heldNetEditorModule.ModuleID == NetModuleID.Reactor
            || _heldNetEditorModule.ModuleData.CanBePowered)
        {
            inEnergyView = true;
            ToggleEnergyViewButtonImage();
        }
    }

    private void ToggleOffEnergyViewBasedOnModule()
    {
        if (inEnergyView && _heldNetEditorModule.ModuleID == NetModuleID.Reactor
            || _heldNetEditorModule.ModuleData.CanBePowered)
        {
            inEnergyView = inEnergyViewParentToggle;
            ToggleEnergyViewButtonImage();
        }
    }

    private void ToggleEnergyViewButtonImage()
    {
        var baseImage = energyViewToggleButton.GetComponent<Image>();
        baseImage.sprite = inEnergyView ? energyViewButtonImageToggled : energyViewButtonImageNormal;
        if (!InputManager.Instance.IsGamepadUsed) EventSystem.current.SetSelectedGameObject(null);
    }

    public void HandleHeldEnergyModule(HexCoordinate coord)
    {
        if (_heldNetEditorModule.ModuleData.CanBePowered)
            _heldNetEditorModule.PlacedLocation = coord;
    }

    private void SetOverLayModulesColourViaHealthMap()
    {
        var values = ShipEditorHealthOverlay.HealthMap.Values.ToList();

        float min = values.Min();
        float max = values.Max();
        float range = max - min;
        float pseudoRange = Mathf.Max(range, 20f);

        netEditorBridgeRef.TotalHealthChangeOverLayColour();
        foreach (NetEditorModule module in _editorModulesMap.Values)
        {
            if (ShipEditorHealthOverlay.HealthMap.TryGetValue(module.PlacedLocation, out var value))
            {
                module.TotalHealthChangeOverLayColour();
            }
        }
    }


    public void PlaceModule(HexCoordinate rootCoord)
    {
        if (_heldNetEditorModule.ModuleID == NetModuleID.Reactor)
        {
            AddPowerToEnergyMap(rootCoord, true, _heldNetEditorModule.ModuleData.EffectRange);
        }

        ToggleOffEnergyViewBasedOnModule();
        _heldNetEditorModule.PlacedLocation = rootCoord;
        foreach (HexCoordinate localCoord in _heldNetEditorModule.LocalCoordinates)
        {
            HexCoordinate coord = rootCoord + localCoord;
            _editorModulesMap[coord] = _heldNetEditorModule;
        }

        if (!joiningEditor)
        {
            weaponGroupManager.AddModuleToWeaponGroup(_heldNetEditorModule, rootCoord);
        }

        ShipEditorHealthOverlay.WriteHealthMap(rootCoord, _heldNetEditorModule.ModuleData.BaseStats.health);
        SetOverLayModulesColourViaHealthMap();
        _heldNetEditorModule.transform.position = hexTransform.Layout.HexToPositionXY(rootCoord).xy0();
        if (_heldNetEditorModule.ModuleData.ModuleCategory != NetModuleCategory.Weapons)
        {
            _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Modules");
        }

        _editorModuleList.Add(_heldNetEditorModule);
        shipEditorStats.GetTotalStats(_editorModuleList, weaponGroupManager);
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
        weaponGroupManager.RemoveModuleFromWeaponGroup(_heldNetEditorModule, moduleToRemove.PlacedLocation);
        ShipEditorHealthOverlay.RemoveHealthMap(moduleToRemove.PlacedLocation);
        SetOverLayModulesColourViaHealthMap();
        shipEditorStats.GetTotalStats(_editorModuleList, weaponGroupManager);
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
        joiningEditor = true;
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

        StartCoroutine(WaitForReconstructShip());
    }

    public IEnumerator WaitForReconstructShip()
    {
        yield return new WaitForSeconds(0.5f);
        joiningEditor = false;
        shipEditorStats.GetTotalStats(_editorModuleList, weaponGroupManager);
    }
}