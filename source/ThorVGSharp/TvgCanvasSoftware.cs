using System.Runtime.InteropServices;

using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Software rendering canvas that renders to a pixel buffer.
/// </summary>
public sealed class TvgCanvasSoftware : TvgCanvas
{
    private GCHandle _pinnedTargetBuffer;
    private bool _hasPinnedTargetBuffer;

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
    /// Sets the target pixel buffer for rendering and pins it for the canvas lifetime (or until another target is set).
    /// </summary>
    /// <param name="buffer">Managed pixel buffer array (RGBA format depending on colorspace)</param>
    /// <param name="stride">Stride (width in pixels) of each row</param>
    /// <param name="width">Width of the buffer</param>
    /// <param name="height">Height of the buffer</param>
    /// <param name="colorspace">Color space format</param>
    /// <exception cref="ArgumentNullException">Thrown when buffer is null.</exception>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTarget(uint[] buffer, uint stride, uint width, uint height, TvgColorSpace colorspace)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        var newPinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        var result = NativeMethods.tvg_swcanvas_set_target(Handle, (uint*)newPinnedBuffer.AddrOfPinnedObject(), stride, width, height, (Tvg_Colorspace)colorspace);
        if (result != Tvg_Result.TVG_RESULT_SUCCESS)
        {
            newPinnedBuffer.Free();
            TvgResultHelper.CheckResult(result, "software canvas set target");
        }

        ReplacePinnedTargetBuffer(newPinnedBuffer);
    }

    /// <summary>
    /// Sets the target pixel buffer for rendering.
    /// </summary>
    /// <param name="buffer">Pixel buffer span (RGBA format depending on colorspace)</param>
    /// <param name="stride">Stride (width in pixels) of each row</param>
    /// <param name="width">Width of the buffer</param>
    /// <param name="height">Height of the buffer</param>
    /// <param name="colorspace">Color space format</param>
    /// <remarks>
    /// This overload is intentionally unsupported because the native canvas retains the buffer pointer after this method returns.
    /// Use <see cref="SetTarget(uint[],uint,uint,uint,TvgColorSpace)"/> or <see cref="SetTarget(IntPtr,uint,uint,uint,TvgColorSpace)"/>.
    /// </remarks>
    /// <exception cref="System.NotSupportedException">Always thrown for safety.</exception>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTarget(ReadOnlySpan<uint> buffer, uint stride, uint width, uint height, TvgColorSpace colorspace)
    {
        throw new System.NotSupportedException("SetTarget(ReadOnlySpan<uint>) cannot guarantee stable native buffer lifetime. Use SetTarget(uint[]) or SetTarget(IntPtr) instead.");
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
        if (result == Tvg_Result.TVG_RESULT_SUCCESS)
        {
            ReleasePinnedTargetBuffer();
        }

        TvgResultHelper.CheckResult(result, "software canvas set target");
    }

    protected override unsafe void Dispose(bool disposing)
    {
        ReleasePinnedTargetBuffer();
        base.Dispose(disposing);
    }

    private void ReleasePinnedTargetBuffer()
    {
        if (_hasPinnedTargetBuffer)
        {
            _pinnedTargetBuffer.Free();
            _hasPinnedTargetBuffer = false;
        }
    }

    private void ReplacePinnedTargetBuffer(GCHandle newPinnedBuffer)
    {
        ReleasePinnedTargetBuffer();
        _pinnedTargetBuffer = newPinnedBuffer;
        _hasPinnedTargetBuffer = true;
    }
}
