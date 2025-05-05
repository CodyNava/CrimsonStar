using UnityEngine;

 public readonly struct HexLayout
{
    private readonly HexOrientation _orientation;
    private readonly Vector2 _size, _origin;

    public HexLayout(HexOrientation orientation, Vector2 size, Vector2 origin)
    {
        _orientation = orientation;
        _size = size;
        _origin = origin;
    }

    public Vector2 HexToPositionXY(HexCoordinate hex)
    {
        HexOrientation m = _orientation;
        float x = (m.F0 * hex.Q + m.F1 * hex.R) * _size.x;
        float y = (m.F2 * hex.Q + m.F3 * hex.R) * _size.y;
        return new Vector2(x + _origin.x, y + _origin.y);
    }

    public HexCoordinate PositionXYToHex(Vector2 position)
    {
        HexOrientation m = _orientation;
        Vector2 pt = new((position.x - _origin.x) / _size.x, (position.y - _origin.y) / _size.y);
        float q = m.B0 * pt.x + m.B1 * pt.y;
        float r = m.B2 * pt.x + m.B3 * pt.y;
        return new FractionalHex(q, r, -q - r).Round();
    }

    public Vector2 HexCornerOffsetXY(int corner)
    {
        HexOrientation m = _orientation;
        float angle = 2f * Mathf.PI * (m.Angle - corner) / 6f;
        return new Vector2(_size.x * Mathf.Cos(angle), _size.y * Mathf.Sin(angle));
    }

    public void DrawGizmos(HexCoordinate hex, Color color)
    {
        Gizmos.color = color;
        Vector2 center = HexToPositionXY(hex);
        for (int i = 0; i < 6; i++)
        {
            Gizmos.DrawLine(center + HexCornerOffsetXY(i), center + HexCornerOffsetXY(i + 1));
        }
    }
}
