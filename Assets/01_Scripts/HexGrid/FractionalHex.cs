using UnityEngine;

public readonly struct FractionalHex
{
    private readonly float _q, _r, _s;

    public FractionalHex(float q, float r, float s)
    {
        _q = q;
        _r = r;
        _s = s;
    }

    public HexCoordinate Round()
    {
        int qi = (int)(Mathf.Round(_q));
        int ri = (int)(Mathf.Round(_r));
        int si = (int)(Mathf.Round(_s));
        float qD = Mathf.Abs(qi - _q);
        float rD = Mathf.Abs(ri - _r);
        float sD = Mathf.Abs(si - _s);
        if (qD > rD && qD > sD)
        {
            qi = -ri - si;
        }
        else if (rD > sD)
        {
            ri = -qi - si;
        }
        else
        {
            si = -qi - ri;
        }
        return new HexCoordinate(qi, ri, si);
    }
    
    public static FractionalHex Lerp(FractionalHex a, FractionalHex b, float t)
    {
        return new FractionalHex(a._q * (1.0f - t) + b._q * t, a._r * (1.0f - t) + b._r * t, a._s * (1.0f - t) + b._s * t);
    }
}
