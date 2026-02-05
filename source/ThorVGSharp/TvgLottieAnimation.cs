using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Represents a Lottie animation with advanced features like slots, markers, and quality control.
/// </summary>
public class TvgLottieAnimation : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the native handle to the Lottie animation object.
    /// </summary>
    internal unsafe _Tvg_Animation* Handle { get; private set; }

    internal unsafe TvgLottieAnimation(_Tvg_Animation* handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Creates a new Lottie animation.
    /// </summary>
    public static unsafe TvgLottieAnimation? Create()
    {
        _Tvg_Animation* handle = NativeMethods.tvg_lottie_animation_new();
        return handle != null ? new TvgLottieAnimation(handle) : null;
    }

    /// <summary>
    /// Generates a unique slot ID for dynamic property assignment.
    /// </summary>
    /// <param name="slot">Slot name</param>
    public unsafe uint GenerateSlot(string slot)
    {
        byte[] slotBytes = System.Text.Encoding.UTF8.GetBytes(slot + '\0');
        fixed (byte* slotPtr = slotBytes)
        {
            return NativeMethods.tvg_lottie_animation_gen_slot(Handle, (sbyte*)slotPtr);
        }
    }

    /// <summary>
    /// Applies a slot configuration.
    /// </summary>
    /// <param name="id">Slot ID</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void ApplySlot(uint id)
    {
        var result = NativeMethods.tvg_lottie_animation_apply_slot(Handle, id);
        TvgResultHelper.CheckResult(result, "lottie animation apply slot");
    }

    /// <summary>
    /// Deletes a previously created slot.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void DeleteSlot(uint slotId)
    {
        var result = NativeMethods.tvg_lottie_animation_del_slot(Handle, slotId);
        TvgResultHelper.CheckResult(result, "lottie animation delete slot");
    }

    /// <summary>
    /// Sets the playback position to a specific marker.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetMarker(string marker)
    {
        byte[] markerBytes = System.Text.Encoding.UTF8.GetBytes(marker + '\0');
        fixed (byte* markerPtr = markerBytes)
        {
            var result = NativeMethods.tvg_lottie_animation_set_marker(Handle, (sbyte*)markerPtr);
            TvgResultHelper.CheckResult(result, "lottie animation set marker");
        }
    }

    /// <summary>
    /// Gets the total number of markers in the animation.
    /// </summary>
    public unsafe uint GetMarkersCount()
    {
        uint cnt;
        NativeMethods.tvg_lottie_animation_get_markers_cnt(Handle, &cnt);
        return cnt;
    }

    /// <summary>
    /// Gets a marker name by index.
    /// </summary>
    public unsafe string? GetMarker(uint index)
    {
        sbyte* markerPtr;
        var result = NativeMethods.tvg_lottie_animation_get_marker(Handle, index, &markerPtr);

        if (result != Tvg_Result.TVG_RESULT_SUCCESS || markerPtr == null)
            return null;

        return System.Runtime.InteropServices.Marshal.PtrToStringUTF8((IntPtr)markerPtr);
    }

    /// <summary>
    /// Tweens between two frames for smooth interpolation.
    /// </summary>
    /// <param name="frameFrom">Starting frame</param>
    /// <param name="frameTo">Ending frame</param>
    /// <param name="progress">Progress between frames (0.0 to 1.0)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Tween(float frameFrom, float frameTo, float progress)
    {
        var result = NativeMethods.tvg_lottie_animation_tween(Handle, frameFrom, frameTo, progress);
        TvgResultHelper.CheckResult(result, "lottie animation tween");
    }

    /// <summary>
    /// Assigns a dynamic property value to a layer.
    /// </summary>
    /// <param name="layer">Layer name</param>
    /// <param name="ix">Index</param>
    /// <param name="var">Variable name</param>
    /// <param name="val">Value to assign</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Assign(string layer, uint ix, string var, float val)
    {
        byte[] layerBytes = System.Text.Encoding.UTF8.GetBytes(layer + '\0');
        byte[] varBytes = System.Text.Encoding.UTF8.GetBytes(var + '\0');

        fixed (byte* layerPtr = layerBytes)
        fixed (byte* varPtr = varBytes)
        {
            var result = NativeMethods.tvg_lottie_animation_assign(Handle, (sbyte*)layerPtr, ix, (sbyte*)varPtr, val);
            TvgResultHelper.CheckResult(result, "lottie animation assign");
        }
    }

    /// <summary>
    /// Sets the rendering quality for the Lottie animation.
    /// </summary>
    /// <param name="quality">Quality level (0-100, higher is better)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetQuality(byte quality)
    {
        var result = NativeMethods.tvg_lottie_animation_set_quality(Handle, quality);
        TvgResultHelper.CheckResult(result, "lottie animation set quality");
    }

    /// <summary>
    /// Sets the current frame number.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetFrame(float frameNumber)
    {
        var result = NativeMethods.tvg_animation_set_frame(Handle, frameNumber);
        TvgResultHelper.CheckResult(result, "lottie animation set frame");
    }

    /// <summary>
    /// Gets the current frame number.
    /// </summary>
    public unsafe float GetFrame()
    {
        float frame;
        NativeMethods.tvg_animation_get_frame(Handle, &frame);
        return frame;
    }

    /// <summary>
    /// Gets the picture at the current frame.
    /// </summary>
    public unsafe TvgPicture? GetPicture()
    {
        _Tvg_Paint* paintHandle = NativeMethods.tvg_animation_get_picture(Handle);

        if (paintHandle == null)
            return null;

        return new TvgPicture(paintHandle);
    }

    /// <summary>
    /// Retrieves the total number of frames in the animation.
    /// </summary>
    /// <returns>The total number of frames in the animation.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    /// <remarks>
    /// Frame numbering starts from 0.
    /// If the Picture is not properly configured, this function will return 0.
    /// </remarks>
    public unsafe float GetTotalFrames()
    {
        float total;
        var result = NativeMethods.tvg_animation_get_total_frame(Handle, &total);
        TvgResultHelper.CheckResult(result, "get total frames");
        return total;
    }

    /// <summary>
    /// Retrieves the duration of the animation in seconds.
    /// </summary>
    /// <returns>The duration of the animation in seconds.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    /// <remarks>
    /// If the Picture is not properly configured, this function will return 0.
    /// </remarks>
    public unsafe float GetDuration()
    {
        float duration;
        var result = NativeMethods.tvg_animation_get_duration(Handle, &duration);
        TvgResultHelper.CheckResult(result, "get duration");
        return duration;
    }

    /// <summary>
    /// Sets the playback segment.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetSegment(float begin, float end)
    {
        var result = NativeMethods.tvg_animation_set_segment(Handle, begin, end);
        TvgResultHelper.CheckResult(result, "lottie animation set segment");
    }

    /// <summary>
    /// Gets the playback segment.
    /// </summary>
    public unsafe (float begin, float end) GetSegment()
    {
        float begin, end;
        NativeMethods.tvg_animation_get_segment(Handle, &begin, &end);
        return (begin, end);
    }

    /// <summary>
    /// Releases the native resources.
    /// </summary>
    protected virtual unsafe void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (Handle != null)
            {
                NativeMethods.tvg_animation_del(Handle);
                Handle = null;
            }
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~TvgLottieAnimation()
    {
        Dispose(false);
    }
}
