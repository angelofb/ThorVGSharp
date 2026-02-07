
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThorVGSharp.Interop;

namespace ThorVGSharp;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct TvgPoint
{
    public readonly float X;
    public readonly float Y;

    public TvgPoint(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Maps a native Tvg_Point to a managed TvgPoint.
    /// Uses zero-copy pointer casting for optimal performance.
    /// </summary>
    /// <param name="nativePoint">The native point to map.</param>
    /// <returns>The mapped managed point.</returns>
    internal static TvgPoint Map(Tvg_Point nativePoint)
    {
        return Unsafe.As<Tvg_Point, TvgPoint>(ref nativePoint);
    }

    /// <summary>
    /// Maps a managed TvgPoint to a native Tvg_Point.
    /// Uses zero-copy pointer casting for optimal performance.
    /// </summary>
    /// <param name="point">The managed point to map.</param>
    /// <returns>The mapped native point.</returns>
    internal static Tvg_Point Map(TvgPoint point)
    {
        return Unsafe.As<TvgPoint, Tvg_Point>(ref point);
    }
}
