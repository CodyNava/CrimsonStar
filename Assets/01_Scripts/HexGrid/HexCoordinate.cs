using System;
using System.Collections.Generic;
using FishNet.CodeGenerating;
using FishNet.Serializing;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable, UseGlobalCustomSerializer]
public struct HexCoordinate : IEquatable<HexCoordinate>
{
    private static readonly HexCoordinate[] DirectionOffsets =
    {
        new(1, 0, -1),
        new(1, -1, 0),
        new(0, -1, 1),
        new(-1, 0, 1),
        new(-1, 1, 0),
        new(0, 1, -1),
    };

    [field: SerializeField]
    public int Q { get; private set; }
    [field: SerializeField]
    public int R { get; private set; }
    [field: SerializeField]
    public int S { get; private set; }

    public static readonly HexCoordinate Zero = new(0, 0);

    public HexCoordinate(int q, int r)
    {
        Q = q;
        R = r;
        S = -q - r;
    }

    public HexCoordinate(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
    }

    public HexCoordinate(Vector3Int coords)
    {
        Q = coords.x;
        R = coords.y;
        S = coords.z;
        Assert.IsTrue(S == -Q - R, "Illegal coordinates found in Module Data Scriptable Object.");
    }
    
    public bool IsWithinBounds(int limit = 6)
    {
        return Mathf.Abs(Q) <= limit && Mathf.Abs(R) <= limit && Mathf.Abs(S) <= limit;
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
        return new HexCoordinate(lhs.Q + rhs.Q, lhs.R + rhs.R, lhs.S + rhs.S);
    }

    public static HexCoordinate operator -(HexCoordinate lhs, HexCoordinate rhs)
    {
        return new HexCoordinate(lhs.Q - rhs.Q, lhs.R - rhs.R, lhs.S - rhs.S);
    }

    public static HexCoordinate operator *(HexCoordinate lhs, HexCoordinate rhs)
    {
        return new HexCoordinate(lhs.Q * rhs.Q, lhs.R * rhs.R, lhs.S * rhs.S);
    }

    public static HexCoordinate Direction(HexDirection direction)
    {
        return DirectionOffsets[(int)direction];
    }

    public static HexCoordinate Neighbor(HexCoordinate hex, HexDirection direction)
    {
        return hex + DirectionOffsets[(int)direction];
    }

    public HexCoordinate RotateClockwise()
    {
        return new HexCoordinate(-R, -S, -Q);
    }

    public HexCoordinate RotateCounterClockwise()
    {
        return new HexCoordinate(-S, -Q, -R);
    }

    // IEnumerable<T> is a return type that lets us use foreach(T var in coord.Neighbors()) to loop through
    // all neighboring coordinates of a HexCoordinate, note the use of "yield return" instead of "return" (similar to coroutines)
    public IEnumerable<HexCoordinate> Neighbors()
    {
        for (HexDirection direction = HexDirection.SouthEast; direction <= HexDirection.South; direction++)
        {
            yield return Neighbor(this, direction);
        }
    }

    public IEnumerable<HexCoordinate> CoordinatesInRange(int range)
    {
        for (int q = -range; q <= range; q++)
        {
            for (int r = Mathf.Max(-range, -q - range); r <= Mathf.Min(range, -q + range); r++)
            {
                int s = -q - r;
                yield return this + new HexCoordinate(q, r, s);
            }
        }
    }

    public bool Equals(HexCoordinate other)
    {
        return Q == other.Q && R == other.R && S == other.S;
    }

    public override bool Equals(object obj)
    {
        return obj is HexCoordinate other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Q, R, S);
    }

    public void DrawGizmos(Color color, float size = 2f, float scale = 1f)
    {
        HexLayout layout = new(HexOrientation.Flat, Vector2.one * size, Vector2.zero);
        layout.DrawGizmos(this, color, scale);
    }
}

public static class HexCoordinateSerialization
{
    public static void WriteCoordinate(this Writer writer, HexCoordinate value)
    {
        writer.WriteInt32(value.Q);
        writer.WriteInt32(value.R);
    }

    public static HexCoordinate ReadCoordinate(this Reader reader)
    {
        return new HexCoordinate(reader.ReadInt32(), reader.ReadInt32());
    }
}
