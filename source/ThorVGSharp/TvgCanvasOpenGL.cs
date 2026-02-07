using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// OpenGL rendering canvas.
/// </summary>
public class TvgCanvasOpenGL : TvgCanvas
{
    internal unsafe TvgCanvasOpenGL(_Tvg_Canvas* handle) : base(handle) { }

    /// <summary>
    /// Creates a new OpenGL canvas.
    /// </summary>
    /// <param name="option">Engine creation option</param>
    public static unsafe TvgCanvasOpenGL Create(TvgEngineOption option = TvgEngineOption.Default)
    {
        _Tvg_Canvas* handle = NativeMethods.tvg_glcanvas_create((Tvg_Engine_Option)option);
        return handle != null ? new TvgCanvasOpenGL(handle) : throw new TvgException("Failed to create OpenGL canvas.");
    }

    /// <summary>
    /// Sets the OpenGL target for rendering.
    /// </summary>
    /// <param name="display">Platform-specific display handle</param>
    /// <param name="surface">Platform-specific surface handle</param>
    /// <param name="context">Platform-specific context handle</param>
    /// <param name="id">OpenGL framebuffer object ID (0 for default)</param>
    /// <param name="width">Width of the rendering area</param>
    /// <param name="height">Height of the rendering area</param>
    /// <param name="colorspace">Color space format</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTarget(IntPtr display, IntPtr surface, IntPtr context, int id, uint width, uint height, TvgColorSpace colorspace)
    {
        var result = NativeMethods.tvg_glcanvas_set_target(Handle, display.ToPointer(), surface.ToPointer(),
            context.ToPointer(), id, width, height, (Tvg_Colorspace)colorspace);
        TvgResultHelper.CheckResult(result, "OpenGL canvas set target");
    }
}
