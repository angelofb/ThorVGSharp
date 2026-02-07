
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThorVGSharp.Interop;

namespace ThorVGSharp;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct TvgColorStop
{
    public readonly float Offset;

    public readonly byte R;
    public readonly byte G;
    public readonly byte B;
    public readonly byte A;

    public TvgColorStop(float offset, byte r, byte g, byte b, byte a)
    {
        Offset = offset;
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Maps a native Tvg_Color_Stop to a managed TvgColorStop.
    /// Uses zero-copy pointer casting for optimal performance.
    /// </summary>
    /// <param name="nativeStop">The native color stop to map.</param>
    /// <returns>The mapped managed color stop.</returns>
    internal static TvgColorStop Map(Tvg_Color_Stop nativeStop)
    {
        return Unsafe.As<Tvg_Color_Stop, TvgColorStop>(ref nativeStop);
    }

    /// <summary>
    /// Maps a managed TvgColorStop to a native Tvg_Color_Stop.
    /// Uses zero-copy pointer casting for optimal performance.
    /// </summary>
    /// <param name="stop">The managed color stop to map.</param>
    /// <returns>The mapped native color stop.</returns>
    internal static Tvg_Color_Stop Map(TvgColorStop stop)
    {
        return Unsafe.As<TvgColorStop, Tvg_Color_Stop>(ref stop);
    }
}
