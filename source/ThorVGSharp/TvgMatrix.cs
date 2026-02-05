
using System.Runtime.CompilerServices;
using ThorVGSharp.Interop;

namespace ThorVGSharp;

public partial struct TvgMatrix
{
    public float E11;
    public float E12;
    public float E13;
    public float E21;
    public float E22;
    public float E23;
    public float E31;
    public float E32;
    public float E33;

    /// <summary>
    /// Maps a managed TvgMatrix to a native Tvg_Matrix.
    /// Uses zero-copy pointer casting for optimal performance.
    /// </summary>
    /// <param name="matrix">The managed matrix to map.</param>
    /// <returns>The mapped native matrix.</returns>
    internal static Tvg_Matrix Map(TvgMatrix matrix)
    {
        return Unsafe.As<TvgMatrix, Tvg_Matrix>(ref matrix);
    }

    /// <summary>
    /// Maps a native Tvg_Matrix to a managed TvgMatrix.
    /// Uses zero-copy pointer casting for optimal performance.
    /// </summary>
    /// <param name="nativeMatrix">The native matrix to map.</param>
    /// <returns>The mapped managed matrix.</returns>
    internal static TvgMatrix Map(Tvg_Matrix nativeMatrix)
    {
        return Unsafe.As<Tvg_Matrix, TvgMatrix>(ref nativeMatrix);
    }
}
