
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
    public struct ColorPreset
    {
        public static ColorPreset GetDefault() => new ColorPreset
        {
            color1 = new Color(0,0,0,1),
            color2 = new Color(0,0,0,1),
            color3 = new Color(0,0,0,1)
        };
        public Color color1, color2, color3;


        public void Deconstruct(out Color colors1, out Color colors2, out Color colors3)
        {
            colors1 = this.color1;
            colors2 = this.color2;
            colors3 = this.color3;
        }
    }
