using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// WebGPU rendering canvas.
/// </summary>
public sealed class TvgCanvasWebGPU : TvgCanvas
{
    internal unsafe TvgCanvasWebGPU(_Tvg_Canvas* handle) : base(handle) { }

    /// <summary>
    /// Creates a new WebGPU canvas.
    /// </summary>
    /// <param name="option">Engine creation option</param>
    public static unsafe TvgCanvasWebGPU Create(TvgEngineOption option = TvgEngineOption.Default)
    {
        _Tvg_Canvas* handle = NativeMethods.tvg_wgcanvas_create((Tvg_Engine_Option)option);
        return handle != null ? new TvgCanvasWebGPU(handle) : throw new TvgException("Failed to create WebGPU canvas.");
    }

    /// <summary>
    /// Sets the WebGPU target for rendering.
    /// </summary>
    /// <param name="device">WebGPU device handle</param>
    /// <param name="instance">WebGPU instance handle</param>
    /// <param name="target">WebGPU target handle (surface or texture)</param>
    /// <param name="width">Width of the rendering area</param>
    /// <param name="height">Height of the rendering area</param>
    /// <param name="colorspace">Color space format</param>
    /// <param name="type">Target type (0 = surface, 1 = texture)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTarget(IntPtr device, IntPtr instance, IntPtr target, uint width, uint height, TvgColorSpace colorspace, int type = 0)
    {
        var result = NativeMethods.tvg_wgcanvas_set_target(Handle, device.ToPointer(), instance.ToPointer(),
            target.ToPointer(), width, height, (Tvg_Colorspace)colorspace, type);
        TvgResultHelper.CheckResult(result, "WebGPU canvas set target");
    }
}
