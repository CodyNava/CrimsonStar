using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using FishNet;
using FishNet.Transporting;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;
using Button = UnityEngine.UI.Button;

public class ShipEditor : MonoBehaviour
{
    [Header("Shader Properties")] 
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int _OutlineThickness = Shader.PropertyToID("_OutLineThickNess");
    private static readonly int Thickness = Shader.PropertyToID("_OutlineThickness");

    [Header("References")] 
    [SerializeField] private Camera editCamera;
    [SerializeField] private HexTransform hexTransform;
    [SerializeField] private ShipEditorStats shipEditorStats;
    [SerializeField] private NetEditorModule netEditorBridgeRef;
    [SerializeField] private ShipEditorWeaponGroups weaponGroupManager;
    [SerializeField] private Material outlineShader;

    [Header("UI - General")]
    [SerializeField] private GameObject noMoneyPopUp;
    [SerializeField] private GameObject cantBePlacedPopUp;
    [SerializeField] private GameObject sellEffect;
    [SerializeField] private Transform noMoneyPopUpSpawnPos;
    [SerializeField] private TextMeshProUGUI cantBePlacedText;
    [SerializeField] private List<GameObject> gamePadSwitches = new();
    [SerializeField] private TextMeshProUGUI wg1Header;
    [SerializeField] private TextMeshProUGUI wg2Header;
    [SerializeField] private TextMeshProUGUI wg3Header;

    [Header("UI - Top Bar")]
    [SerializeField] private List<GameObject> playerWidget;
    [SerializeField] private TMP_Text topBarRounds;
    [SerializeField] private GameObject topBarContainer;
    [SerializeField] private int maxNameChars;
    
    [Header("UI - Ready Blockade")]
    public GameObject blockingPlane;

    [Header("UI - Health View")]
    [SerializeField] private TextMeshProUGUI healthViewModeText;
    [SerializeField] private Button healthViewToggleButton;
    [SerializeField] private Sprite healthViewImageToggled;
    [SerializeField] private Sprite healthViewImageNormal;
    [HideInInspector] public bool inHealthView;
    [HideInInspector] public bool inPercentageHealthView;

    [Header("UI - Energy View")]
    [SerializeField] private Button energyViewToggleButton;
    [SerializeField] private Sprite energyViewButtonImageToggled;
    [SerializeField] private Sprite energyViewButtonImageNormal;
    [HideInInspector] public bool inEnergyView;
    [HideInInspector] public bool inEnergyViewParentToggle;

    [Header("Input")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private GameObject lastSelected;
    [SerializeField] private GameObject forceSelected;
    [SerializeField] private float moduleMoveSpeedGp;
    [SerializeField] private bool gamepadEnabled;
    [HideInInspector] public bool moduleSpawnedInGp;
    [HideInInspector] public bool moduleFirstSelectedGp;

    [Header("State")]
    [SerializeField] private bool isReady;
    public bool IsReady => isReady;
    private bool _gamePadSwitchSelectToggle;
    [HideInInspector] public bool joiningEditor;
    [HideInInspector] public int tempRotation;

    [Header("Color Presets")]
    [SerializeField] private ColorPresetData presetData;

    [Header("Audio")]
    [SerializeField] private FMODUnity.EventReference modulePlacedEvent;
    [SerializeField] private FMODUnity.EventReference moduleRefundEvent;
    [SerializeField] private FMODUnity.EventReference moduleBuyEvent;

    [Header("Runtime Data")] 
    public NetMatchPlayer PlayerData { get; private set; }
    private readonly Dictionary<HexCoordinate, NetEditorModule> _blockedCoordinates = new();
    private readonly Dictionary<HexCoordinate, NetEditorModule> _editorModulesMap = new();
    [SerializedDictionary("EnergyMap")] public SerializedDictionary<HexCoordinate, int> energyMap = new();
    private NetEditorModule _heldNetEditorModule;
    private List<NetEditorModule> _editorModuleList;
    public IReadOnlyList<NetEditorModule> EditorModuleList => _editorModuleList;
    private Color _originalOutlineShaderColor;
    private float _originalOutlineShaderStrenght;
    
    private void Update()
    {
        if (InputManager.Instance.IsGamepadUsed)
        {
            if (moduleSpawnedInGp)
            {
                moduleSpawnedInGp = false;
                return;
            }

            if (!_gamePadSwitchSelectToggle)
            {
                _gamePadSwitchSelectToggle = true;
                EventSystem.current.SetSelectedGameObject(lastSelected ? lastSelected : forceSelected);
            }

            ModueHoldingGamePad();
        }
        else
        {
            _gamePadSwitchSelectToggle = false;
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
        ColorPresetButton.ColorSelected -= SetPresetName;
        ColorPresetButton.ColorSelected += SetPresetName;

        InstanceFinder.ClientManager.RegisterBroadcast<NetShipEditorBroadcasts.ShipEditorUpdate>(OnServerUpdate);

        _editorModuleList = new List<NetEditorModule> // collection initialization syntax uwu
        {
            netEditorBridgeRef
        };
        _blockedCoordinates.Add(HexCoordinate.Zero, netEditorBridgeRef);
        _blockedCoordinates.Add(HexCoordinate.Direction(HexDirection.North), netEditorBridgeRef);
        StartCoroutine(LinkPlayerRoutine());
        shipEditorStats.GetTotalStats(_editorModuleList, weaponGroupManager);
        // energyViewToggleButton.onClick.AddListener(ToggleEnergyView);
        _originalOutlineShaderColor = outlineShader.GetColor(OutlineColor);
        _originalOutlineShaderStrenght = outlineShader.GetFloat("_OutlineThickness");
        inHealthView = false;
        inPercentageHealthView = false;
        blockingPlane.gameObject.SetActive(false);
        SetOverLayModulesColourViaHealthMap();
    }

    private void OnServerUpdate(NetShipEditorBroadcasts.ShipEditorUpdate data, Channel channel = Channel.Reliable)
    {
        int count = data.names.Length;

        for (int i = 0; i < count; i++)
        {
            playerWidget[i].SetActive(true);
            if (data.names[i] == null) continue;
            var playerLabel = playerWidget[i].GetComponentInChildren<TextMeshProUGUI>();
            playerLabel.text = data.names[i];
            playerLabel.color = data.readyState[i] ? Color.green : Color.white;
        }

        topBarRounds.text = $"Rounds: {data.currentRound}/{data.maxRounds}";
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
                    if (string.IsNullOrEmpty(PlayerData.SelectedPreset.Value)) PlayerData.C_SetPresetName("Default");
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void OnDestroy()
    {
        ModuleSelectionButton.ModuleSelected -= SpawnPart;
        ColorPresetButton.ColorSelected -= SetPresetName;
        InstanceFinder.ClientManager.UnregisterBroadcast<NetShipEditorBroadcasts.ShipEditorUpdate>(OnServerUpdate);
    }

    public void SpawnPart(NetModuleID moduleID)
    {
        bool success = TrySpawnPart(moduleID);
        if (!success)
        {
            Instantiate(noMoneyPopUp, noMoneyPopUpSpawnPos);
        }
    }

    public void SetPresetName(string presetName)
    {
        PlayerData.C_SetPresetName(presetName);
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
        outlineShader.SetFloat("_OutlineThickness", _originalOutlineShaderStrenght * 2.5f);
        yield return new WaitForSeconds(0.4f);
        outlineShader.SetColor(OutlineColor, _originalOutlineShaderColor);
        outlineShader.SetFloat("_OutlineThickness", _originalOutlineShaderStrenght);
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
            moduleSpawnedInGp = true;
            Vector3 spawnVec = new Vector3(0f, 0f, 0f);
            _heldNetEditorModule =
                Instantiate(moduleID.GetModuleData().ShipEditorPrefab, spawnVec, Quaternion.identity);
        }
        else
        {
            _heldNetEditorModule = Instantiate(moduleID.GetModuleData().ShipEditorPrefab,
                editCamera.ScreenToWorldPoint(Input.mousePosition).xy(), Quaternion.identity);
            _heldNetEditorModule.TotalHealthChangeOverLayColour();
        }

        FMODUnity.RuntimeManager.PlayOneShot(moduleBuyEvent, _heldNetEditorModule.transform.position);
        _heldNetEditorModule.Initialize();
        PlayerData.C_PayForModule(moduleID);
        _heldNetEditorModule.VisualTransform.gameObject.layer = LayerMask.NameToLayer("Outline");
        for (int i = 0; i < tempRotation; i++)
        {
            if (_heldNetEditorModule.ModuleData.CanRotate) _heldNetEditorModule.C_RotateClockwise();
        }

        return true;
    }

    private void ToggleGamePadSwitches()
    {
        foreach (var switches in gamePadSwitches)
        {
            switches.SetActive(InputManager.Instance.IsGamepadUsed);
        }

        wg1Header.text = InputManager.Instance.IsGamepadUsed
            ? $"Weapon Group 1 '{GetShortGamepadBinding(Keybinds.Actions.Player.Attack)}'"
            : $"Weapon Group 1 '{GetShortMouseAndKeyBoardBinding(Keybinds.Actions.Player.Attack)}'";
        wg2Header.text = InputManager.Instance.IsGamepadUsed
            ? $"Weapon Group 2 '{GetShortGamepadBinding(Keybinds.Actions.Player.Attack2)}'"
            : $"Weapon Group 2 '{GetShortMouseAndKeyBoardBinding(Keybinds.Actions.Player.Attack2)}'";
        wg3Header.text = InputManager.Instance.IsGamepadUsed
            ? $"Weapon Group 3 '{GetShortGamepadBinding(Keybinds.Actions.Player.Attack3)}'"
            : $"Weapon Group 3 '{GetShortMouseAndKeyBoardBinding(Keybinds.Actions.Player.Attack3)}'";
    }


    string GetShortGamepadBinding(InputAction action)
    {
        var binding = action.bindings.FirstOrDefault(b =>
            b.effectivePath != null && b.effectivePath.Contains("Gamepad"));
        if (binding != default)
        {
            var controlName = binding.effectivePath.Split('/').Last();
            return ControlsShortFormsLib.GamepadShortNames.GetValueOrDefault(controlName, controlName);
        }

        return "404";
    }

    string GetShortMouseAndKeyBoardBinding(InputAction action)
    {
        var mouseBinding = action.bindings.FirstOrDefault(b =>
            b.effectivePath != null && b.effectivePath.Contains("Mouse"));

        if (mouseBinding != default)
        {
            var mouseControlName = mouseBinding.effectivePath.Split('/').Last();
            return ControlsShortFormsLib.KeyboardMouseShortNames.GetValueOrDefault(mouseControlName, mouseControlName);
        }

        var keyboardBinding = action.bindings.FirstOrDefault(b =>
            b.effectivePath != null && b.effectivePath.Contains("Keyboard"));

        if (keyboardBinding != default)
        {
            var keyboardControlName = keyboardBinding.effectivePath.Split('/').Last();
            return ControlsShortFormsLib.KeyboardMouseShortNames.GetValueOrDefault(keyboardControlName,
                keyboardControlName);
        }

        return "404";
    }


    private void ModueHoldingGamePad() // Important for radial menu
    {
        if (!gamepadEnabled) return;
        var t = Time.deltaTime;
        var v = moduleMoveSpeedGp;
        moveInput = Keybinds.Actions.ShipEditor.MoveModule.ReadValue<Vector2>();
        moveInput.Normalize();

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
                if (moduleFirstSelectedGp)
                {
                    moduleFirstSelectedGp = false;
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
                Instantiate(sellEffect, mousePosWorld.xy(), Quaternion.identity);
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
            return false;
        }

        foreach (HexCoordinate localCoord in _heldNetEditorModule.LocalCoordinates)
        {
            HexCoordinate coord = rootCoord + localCoord;
            var moduleID = _heldNetEditorModule.ModuleID;
            var condition = GetPlacementConditionForModule(moduleID);
            if (PlayerData.ModuleStorage.SC_IsCoordinateOccupied(coord) || condition(rootCoord) ||
                CantPlaceHullPlating(coord, _heldNetEditorModule) ||
                IsThrusterAbove(coord) || !coord.IsWithinBounds())
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
        if (moduleID.GetModuleData().ModuleCategory == NetModuleCategory.Engines)
        {
            return IsSomethingBelowThruster;
        }

        return IsThrusterAbove;
    }

    private bool IsThrusterAbove(HexCoordinate coord)
    {
        const int maxDistance = 14;
        for (var i = 0; i < maxDistance; i++)
        {
            coord = HexCoordinate.Neighbor(coord, HexDirection.North);
            if (_editorModulesMap.TryGetValue(coord, out var module))
            {
                if (module.ModuleData.ModuleCategory == NetModuleCategory.Engines)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static readonly Dictionary<int, HexDirection[]> HullDirections = new()
    {
        { 0, new[] { HexDirection.South, HexDirection.SouthEast, HexDirection.SouthWest } },
        { 1, new[] { HexDirection.SouthWest, HexDirection.South, HexDirection.NorthWest } },
        { 2, new[] { HexDirection.NorthWest, HexDirection.SouthWest, HexDirection.North } },
        { 3, new[] { HexDirection.North, HexDirection.NorthWest, HexDirection.NorthEast } },
        { 4, new[] { HexDirection.NorthEast, HexDirection.North, HexDirection.SouthEast } },
        { 5, new[] { HexDirection.SouthEast, HexDirection.NorthEast, HexDirection.South } }
    };

    private bool CantPlaceHullPlating(HexCoordinate coordinate, NetEditorModule module)
    {
        if (module.ModuleID != NetModuleID.HullPlating && module.ModuleID != NetModuleID.HullFinish)
            return false;

        var hullFinish = module.ModuleID == NetModuleID.HullFinish;
        var rotation = _heldNetEditorModule.PlacedRotation;

        if (!HullDirections.TryGetValue(rotation, out var directions))
        {
            return false;
        }

        int directionsToCheck = hullFinish ? 2 : 3;

        for (int i = 0; i < directionsToCheck; i++)
        {
            var neighborCoord = HexCoordinate.Neighbor(coordinate, directions[i]);
            if (_editorModulesMap.TryGetValue(neighborCoord, out var neighborModule) ||
                _blockedCoordinates.ContainsKey(neighborCoord))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsSomethingBelowThruster(HexCoordinate coord)
    {
        const int maxDistance = 14;
        for (var i = 0; i < maxDistance; i++)
        {
            coord = HexCoordinate.Neighbor(coord, HexDirection.South);
            if (_editorModulesMap.TryGetValue(coord, out var value))
            {
                return true;
            }

            if (_blockedCoordinates.ContainsKey(coord))
            {
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

    public bool CheckIfPowered(List<HexCoordinate> coordinates)
    {
        foreach (var coord in coordinates)
        {
            if (energyMap.GetValueOrDefault(coord) >= 1)
            {
                return true;
            }
        }

        return false;
    }

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

        tempRotation = _heldNetEditorModule.PlacedRotation;

        _editorModuleList.Add(_heldNetEditorModule);
        shipEditorStats.GetTotalStats(_editorModuleList, weaponGroupManager);
        _heldNetEditorModule = null;
        if (!InputManager.Instance.IsGamepadUsed) return;
        EventSystem.current.SetSelectedGameObject(lastSelected);
    }

    public void RemoveModule(NetEditorModule moduleToRemove)
    {
        if (InputManager.Instance.IsGamepadUsed)
        {
            _heldNetEditorModule = moduleToRemove;
        }
        Instantiate(sellEffect);
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
        if (isReady)
        {
            if (PlayerData.C_SignalUnready())
            {
                gamepadEnabled = true;
                isReady = false;
            }

            return;
        }

        if (PlayerData.C_SignalReady())
        {
            gamepadEnabled = false;
            isReady = true;
        }
    }

    public void ReconstructShip(IEnumerable<ModulePlacementData> uniqueModules)
    {
        joiningEditor = true;
        blockingPlane.gameObject.SetActive(false);
        gamepadEnabled = true;
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

        isReady = false;
        StartCoroutine(WaitForReconstructShip());
    }

    public IEnumerator WaitForReconstructShip()
    {
        yield return new WaitForSeconds(0.5f);
        joiningEditor = false;
        shipEditorStats.GetTotalStats(_editorModuleList, weaponGroupManager);
    }
}