using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Provides frame control for animated content.
/// </summary>
public class TvgAnimation : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the native handle to the animation object.
    /// </summary>
    internal unsafe _Tvg_Animation* Handle { get; private set; }

    internal unsafe TvgAnimation(_Tvg_Animation* handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Creates a new animation.
    /// </summary>
    public static unsafe TvgAnimation Create()
    {
        _Tvg_Animation* handle = NativeMethods.tvg_animation_new();
        return handle != null ? new TvgAnimation(handle) : throw new TvgException("Failed to create animation.");
    }

    /// <summary>
    /// Sets the current frame number.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetFrame(float frameNumber)
    {
        var result = NativeMethods.tvg_animation_set_frame(Handle, frameNumber);
        TvgResultHelper.CheckResult(result, "animation set frame");
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
    /// <param name="begin">Start frame</param>
    /// <param name="end">End frame</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetSegment(float begin, float end)
    {
        var result = NativeMethods.tvg_animation_set_segment(Handle, begin, end);
        TvgResultHelper.CheckResult(result, "animation set segment");
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
    ~TvgAnimation()
    {
        Dispose(false);
    }
}
