
using System.Runtime.CompilerServices;
using ThorVGSharp.Interop;

namespace ThorVGSharp;

public partial struct TvgColorStop
{
    public float Offset;

    public byte R;
    public byte G;
    public byte B;
    public byte A;

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