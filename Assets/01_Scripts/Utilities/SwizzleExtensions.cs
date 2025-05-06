using System.Runtime.CompilerServices;
using UnityEngine;

public static class SwizzleExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 xy(this Vector2 v) => new(v.x, v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 xy0(this Vector2 v) => new(v.x, v.y, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 xy(this Vector3 v) => new(v.x, v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 xz(this Vector3 v) => new(v.x, v.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 yz(this Vector3 v) => new(v.y, v.z);
}
