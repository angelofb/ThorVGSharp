using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Represents a linear gradient fill.
/// </summary>
public sealed class TvgLinearGradient : TvgFill
{
    internal unsafe TvgLinearGradient(_Tvg_Gradient* handle) : base(handle) { }

    /// <summary>
    /// Creates a new linear gradient.
    /// </summary>
    public static unsafe TvgLinearGradient Create()
    {
        _Tvg_Gradient* handle = NativeMethods.tvg_linear_gradient_new();
        return handle != null ? new TvgLinearGradient(handle) : throw new TvgException("Failed to create linear gradient.");
    }

    /// <summary>
    /// Sets the linear gradient coordinates.
    /// </summary>
    /// <param name="x1">Start X coordinate</param>
    /// <param name="y1">Start Y coordinate</param>
    /// <param name="x2">End X coordinate</param>
    /// <param name="y2">End Y coordinate</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetLinear(float x1, float y1, float x2, float y2)
    {
        var result = NativeMethods.tvg_linear_gradient_set(Handle, x1, y1, x2, y2);
        TvgResultHelper.CheckResult(result, "linear gradient set");
    }

    /// <summary>
    /// Gets the linear gradient coordinates.
    /// </summary>
    public unsafe (float x1, float y1, float x2, float y2) GetLinear()
    {
        float x1, y1, x2, y2;
        NativeMethods.tvg_linear_gradient_get(Handle, &x1, &y1, &x2, &y2);
        return (x1, y1, x2, y2);
    }
}
