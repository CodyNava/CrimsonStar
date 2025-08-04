using System.Collections.Generic;
using UnityEngine;

    public static class ControlsShortFormsLib
    {
        
        public static readonly Dictionary<string, string> GamepadShortNames = new()
        {
            { "rightTrigger", "RT" },
            { "leftTrigger", "LT" },
            { "rightShoulder", "RB" },
            { "leftShoulder", "LB" },
            { "buttonSouth", "A" },
            { "buttonEast", "B" },
            { "buttonWest", "X" },
            { "buttonNorth", "Y" },
            { "start", "Start" },
            { "select", "Select" },
            { "leftStickPress", "L3" },
            { "rightStickPress", "R3" },
            { "dpad/up", "D-Up" },
            { "dpad/down", "D-Down" },
            { "dpad/left", "D-Left" },
            { "dpad/right", "D-Right" }
        };
        
        public static readonly Dictionary<string, string> KeyboardMouseShortNames = new()
        {
            // Maus
            { "leftButton", "LMB" },
            { "rightButton", "RMB" },
            { "middleButton", "MMB" },
            { "forwardButton", "MB4" },
            { "backButton", "MB5" },
            { "scroll/up", "Scroll Up" },
            { "scroll/down", "Scroll Down" },

            // Pfeiltasten
            { "arrowUp", "↑" },
            { "arrowDown", "↓" },
            { "arrowLeft", "←" },
            { "arrowRight", "→" },

            // Standard-Tasten
            { "space", "SPACE" },
            { "enter", "ENTER" },
            { "escape", "ESC" },
            { "backspace", "BACK" },
            { "tab", "TAB" },
            { "leftShift", "LSHIFT" },
            { "rightShift", "RSHIFT" },
            { "leftCtrl", "LCTRL" },
            { "rightCtrl", "RCTRL" },
            { "leftAlt", "LALT" },
            { "rightAlt", "RALT" },

            // F-Tasten
            { "f1", "F1" },
            { "f2", "F2" },
            { "f3", "F3" },
            { "f4", "F4" },
            { "f5", "F5" },
            { "f6", "F6" },
            { "f7", "F7" },
            { "f8", "F8" },
            { "f9", "F9" },
            { "f10", "F10" },
            { "f11", "F11" },
            { "f12", "F12" },

            // Zahlentasten (oben)
            { "digit1", "1" },
            { "digit2", "2" },
            { "digit3", "3" },
            { "digit4", "4" },
            { "digit5", "5" },
            { "digit6", "6" },
            { "digit7", "7" },
            { "digit8", "8" },
            { "digit9", "9" },
            { "digit0", "0" },

            // WASD und Co.
            { "w", "W" },
            { "a", "A" },
            { "s", "S" },
            { "d", "D" },
            { "q", "Q" },
            { "e", "E" },
            { "r", "R" },
            { "f", "F" },
            { "z", "Z" },
            { "x", "X" },
            { "c", "C" },
            { "v", "V" },
            { "shift", "SHIFT" },
            { "ctrl", "CTRL" },
            { "alt", "ALT" }
        };
    }
