using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Represents a radial gradient fill.
/// </summary>
public class TvgRadialGradient : TvgFill
{
    internal unsafe TvgRadialGradient(_Tvg_Gradient* handle) : base(handle) { }

    /// <summary>
    /// Creates a new radial gradient.
    /// </summary>
    public static unsafe TvgRadialGradient Create()
    {
        _Tvg_Gradient* handle = NativeMethods.tvg_radial_gradient_new();
        return handle != null ? new TvgRadialGradient(handle) : throw new TvgException("Failed to create radial gradient.");
    }

    /// <summary>
    /// Sets the radial gradient parameters.
    /// </summary>
    /// <param name="cx">Center X coordinate</param>
    /// <param name="cy">Center Y coordinate</param>
    /// <param name="radius">Radius of the gradient</param>
    /// <param name="fx">Focal point X coordinate (optional)</param>
    /// <param name="fy">Focal point Y coordinate (optional)</param>
    /// <param name="focalRadius">Focal radius (optional)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetRadial(float cx, float cy, float radius, float fx = 0, float fy = 0, float focalRadius = 0)
    {
        var result = NativeMethods.tvg_radial_gradient_set(Handle, cx, cy, radius, fx, fy, focalRadius);
        TvgResultHelper.CheckResult(result, "radial gradient set");
    }

    /// <summary>
    /// Gets the radial gradient parameters.
    /// </summary>
    public unsafe (float cx, float cy, float radius, float fx, float fy, float focalRadius) GetRadial()
    {
        float cx, cy, radius, fx, fy, focalRadius;
        NativeMethods.tvg_radial_gradient_get(Handle, &cx, &cy, &radius, &fx, &fy, &focalRadius);
        return (cx, cy, radius, fx, fy, focalRadius);
    }
}
