using System;

public readonly struct HexCoordinate : IEquatable<HexCoordinate>
{
    private readonly int _q, _r, _s;

    private static readonly HexCoordinate[] DirectionOffsets =
    {
        new(1, 0, -1),
        new(1, -1, 0),
        new(0, -1, 1),
        new(-1, 0, 1),
        new(-1, 1, 0),
        new(0, 1, -1),
    };

    public int Q => _q;
    public int R => _r;
    public int S => _s;
    
    public HexCoordinate(int q, int r)
    {
        _q = q;
        _r = r;
        _s = -q - r;
    }

    public HexCoordinate(int q, int r, int s)
    {
        _q = q;
        _r = r;
        _s = s;
    }

    public static bool operator ==(HexCoordinate lhs, HexCoordinate rhs)
    {
        return lhs.Q == rhs.Q && lhs.R == rhs.R;
    }

    public static bool operator !=(HexCoordinate lhs, HexCoordinate rhs)
    {
        return !(lhs == rhs);
    }

    public static HexCoordinate operator +(HexCoordinate lhs, HexCoordinate rhs)
    {
        return new HexCoordinate(lhs._q + rhs._q, lhs._r + rhs._r, lhs._s + rhs._s);
    }
    
    public static HexCoordinate operator -(HexCoordinate lhs, HexCoordinate rhs)
    {
        return new HexCoordinate(lhs._q - rhs._q, lhs._r - rhs._r, lhs._s - rhs._s);
    }
    
    public static HexCoordinate operator *(HexCoordinate lhs, HexCoordinate rhs)
    {
        return new HexCoordinate(lhs._q * rhs._q, lhs._r * rhs._r, lhs._s * rhs._s);
    }

    public static HexCoordinate Direction(HexDirection direction)
    {
        return DirectionOffsets[(int)direction];
    }

    public static HexCoordinate Neighbor(HexCoordinate hex, HexDirection direction)
    {
        return hex + DirectionOffsets[(int)direction];
    }
    
    public bool Equals(HexCoordinate other)
    {
        return _q == other._q && _r == other._r && _s == other._s;
    }

    public override bool Equals(object obj)
    {
        return obj is HexCoordinate other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_q, _r, _s);
    }
}
