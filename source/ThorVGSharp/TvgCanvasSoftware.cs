using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Software rendering canvas that renders to a pixel buffer.
/// </summary>
public class TvgCanvasSoftware : TvgCanvas
{
    internal unsafe TvgCanvasSoftware(_Tvg_Canvas* handle) : base(handle) { }

    /// <summary>
    /// Creates a new software canvas.
    /// </summary>
    /// <param name="option">Engine creation option</param>
    public static unsafe TvgCanvasSoftware Create(TvgEngineOption option = TvgEngineOption.Default)
    {
        _Tvg_Canvas* handle = NativeMethods.tvg_swcanvas_create((Tvg_Engine_Option)option);
        return handle != null ? new TvgCanvasSoftware(handle) : throw new TvgException("Failed to create software canvas.");
    }

    /// <summary>
    /// Sets the target pixel buffer for rendering.
    /// </summary>
    /// <param name="buffer">Pixel buffer span (RGBA format depending on colorspace)</param>
    /// <param name="stride">Stride (width in pixels) of each row</param>
    /// <param name="width">Width of the buffer</param>
    /// <param name="height">Height of the buffer</param>
    /// <param name="colorspace">Color space format</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTarget(ReadOnlySpan<uint> buffer, uint stride, uint width, uint height, TvgColorSpace colorspace)
    {
        fixed (uint* bufferPtr = buffer)
        {
            var result = NativeMethods.tvg_swcanvas_set_target(Handle, bufferPtr, stride, width, height, (Tvg_Colorspace)colorspace);
            TvgResultHelper.CheckResult(result, "software canvas set target");
        }
    }

    /// <summary>
    /// Sets the target pixel buffer for rendering using a pointer.
    /// </summary>
    /// <param name="buffer">Pointer to pixel buffer</param>
    /// <param name="stride">Stride (width in pixels) of each row</param>
    /// <param name="width">Width of the buffer</param>
    /// <param name="height">Height of the buffer</param>
    /// <param name="colorspace">Color space format</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTarget(IntPtr buffer, uint stride, uint width, uint height, TvgColorSpace colorspace)
    {
        var result = NativeMethods.tvg_swcanvas_set_target(Handle, (uint*)buffer, stride, width, height, (Tvg_Colorspace)colorspace);
        TvgResultHelper.CheckResult(result, "software canvas set target");
    }
}
