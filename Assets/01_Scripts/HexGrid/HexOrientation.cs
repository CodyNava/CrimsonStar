using UnityEngine;

public readonly struct HexOrientation
{
    public float F0 { get; }
    public float F1 { get; }
    public float F2 { get; }
    public float F3 { get; }
    public float B0 { get; }
    public float B1 { get; }
    public float B2 { get; }
    public float B3 { get; }
    public float Angle { get; }

    private HexOrientation(Vector4 forward, Vector4 backward, float angle)
    {
        F0 = forward.x;
        F1 = forward.y;
        F2 = forward.z;
        F3 = forward.w;
        B0 = backward.x;
        B1 = backward.y;
        B2 = backward.z;
        B3 = backward.w;
        Angle = angle;
    }

    public static readonly HexOrientation Pointy = new(
        new Vector4(Mathf.Sqrt(3f), Mathf.Sqrt(3f) / 2f, 0f, 1.5f),
        new Vector4(Mathf.Sqrt(3f) / 3f, -1f / 3f, 0f, 2f / 3f), 0.5f);
    public static readonly HexOrientation Flat = new(
        new Vector4(1.5f, 0f, Mathf.Sqrt(3f) / 2f, Mathf.Sqrt(3f)),
        new Vector4(2f / 3f, 0f, -1f / 3f, Mathf.Sqrt(3f) / 3f), 0f);
}
