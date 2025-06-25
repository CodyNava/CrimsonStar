using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class NetDynamicModule : NetEditorModule
{
    // [SerializeField] private SerializedDictionary<int, bool> _filledNeighbour;
    [SerializeField] private SerializedDictionary<HexDirection, GameObject> _meshes;
    private int _filledNeighbourMask;
    
    
    // Handling edge cases
    private readonly Dictionary<int, int> NeighbourMapping = new()
    {
        {0b010100, 0b011100},
        {0b001010, 0b001110},
        {0b000101, 0b000111},
        {0b100010, 0b100011},
        {0b010001, 0b110001},
        {0b101000, 0b111000},

        {0b110100, 0b111100},
        {0b011010, 0b011110},
        {0b001101, 0b001111},
        {0b100110, 0b100111},
        {0b010011, 0b110011},
        {0b101001, 0b111001},
        
        {0b110010, 0b110011},
        {0b011001, 0b111001},
        {0b101100, 0b111100},
        {0b010110, 0b011110},
        {0b001011, 0b001111},
        {0b100101, 0b100111},
        
        {0b110110, 0b111111},
        {0b011011, 0b111111},
        {0b101101, 0b111111},
        
        {0b101110, 0b111111},
        {0b011101, 0b111111},
        {0b111010, 0b111111},
        {0b110101, 0b111111},
        {0b101011, 0b111111},
        {0b010111, 0b111111},

        {0b111110, 0b111111},
        {0b011111, 0b111111},
        {0b101111, 0b111111},
        {0b110111, 0b111111},
        {0b111011, 0b111111},
        {0b111101, 0b111111}
    };

    public void Awake()
    {
        ResetFilledNeighbour();
    }

    public override void Initialize(ShipEditor editor)
    { 
        base.Initialize(editor);
        _shipEditor.OnPlacedModule += OnShipEditorPlacedModule;
        _shipEditor.OnRemovedModule += OnShipEditorRemovedModule;
    }

    public void OnDisable()
    {
        _shipEditor.OnPlacedModule -= OnShipEditorPlacedModule;
        _shipEditor.OnRemovedModule -= OnShipEditorRemovedModule;
    }

    public override void OnPickedUp()
    {
        base.OnPickedUp();
        ResetFilledNeighbour();
        
        UpdateArmorVisuals();
    }

    public override void OnPlacedDown()
    {
        base.OnPlacedDown();
        
        for (HexDirection direction = HexDirection.SouthEast; direction <= HexDirection.South; direction++)
        {
            HexCoordinate coord = PlacedLocation + HexCoordinate.Direction(direction);
            if (_shipEditor.EditorModulesMap.ContainsKey(coord))
            {
                _filledNeighbourMask |= (1 << (int)direction);
            }
            else
            {
                _filledNeighbourMask &= ~(1 << (int)direction);
            }
        }
        
        UpdateArmorVisuals();
    }

    private void OnShipEditorPlacedModule(HexCoordinate coord, NetModuleID moduleID)
    {
        if (coord == PlacedLocation) return;
        
        if (HexCoordinate.Distance(PlacedLocation, coord) != 1) return;
        if ((coord - PlacedLocation).TryToDirection(out HexDirection neighbourDir))
        {
            _filledNeighbourMask |= (1 << (int)neighbourDir);
        }
        
        UpdateArmorVisuals();
    }

    private void OnShipEditorRemovedModule(HexCoordinate coord, NetModuleID moduleID)
    {
        if (coord == PlacedLocation) return;
        
        if (HexCoordinate.Distance(PlacedLocation, coord) != 1) return;
        if ((coord - PlacedLocation).TryToDirection(out HexDirection neighbourDir))
        {
            _filledNeighbourMask &= ~(1 << (int)neighbourDir);
        }
        
        UpdateArmorVisuals();
    }

    public void UpdateArmorVisuals()
    {
        if (AnyNeighbourFilled())
        {
            int mask = _filledNeighbourMask;
            if (NeighbourMapping.ContainsKey(mask)) mask = NeighbourMapping[mask];
            // MapEdgeCaseMask(_filledNeighbourMask, out int mask);
            
            foreach (KeyValuePair<HexDirection,GameObject> mesh in _meshes)
            {
                mesh.Value.SetActive((mask & (1 << (int)mesh.Key)) > 0);
            }   
        }
        else
        {
            foreach (KeyValuePair<HexDirection,GameObject> mesh in _meshes)
            {
                mesh.Value.SetActive(true);
            }
        }
    }


    private bool AnyNeighbourFilled()
    {
        return _filledNeighbourMask > 0;
    }

    private void ResetFilledNeighbour()
    {
        _filledNeighbourMask = 0;
    }

    private bool MapEdgeCaseMask(int input, out int mappedMask)
    {
        // Check for each EdgeCase entry
        foreach (var mapping in NeighbourMapping)
        {
            // We double the mapping mask, to wrap the falling bits to the front, when right shifting
            int mask = (mapping.Key << 6) | mapping.Key;
            int output = (mapping.Value << 6) | mapping.Value;

            // We will shift the mapping mask 6 times, but the first one is 0, that checks the initial EdgeCase mask
            // If we match the mask by Bitwise AND and comparing with the initial value, we early return a true
            for (int i = 0; i < 6; ++i)
            {
                int checkingMask = mask >> i;
                if ((input & checkingMask) != input) continue;
                
                mappedMask = (output >> i) & 0b111111;
                return true;
            }
        }

        mappedMask = input;
        return false;
    }
}
