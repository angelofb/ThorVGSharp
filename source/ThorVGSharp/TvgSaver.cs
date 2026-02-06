using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Provides functionality to save paint objects and animations to files.
/// </summary>
public class TvgSaver : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the native handle to the saver object.
    /// </summary>
    internal unsafe _Tvg_Saver* Handle { get; private set; }

    internal unsafe TvgSaver(_Tvg_Saver* handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Creates a new saver.
    /// </summary>
    public static unsafe TvgSaver? Create()
    {
        _Tvg_Saver* handle = NativeMethods.tvg_saver_new();
        return handle != null ? new TvgSaver(handle) : null;
    }

    /// <summary>
    /// Saves a paint object to a file.
    /// </summary>
    /// <param name="paint">Paint object to save</param>
    /// <param name="path">File path</param>
    /// <param name="quality">Quality level (0-100, higher is better)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Save(TvgPaint paint, string path, uint quality = 100)
    {
        ArgumentNullException.ThrowIfNull(paint);

        byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(path + '\0');
        fixed (byte* pathPtr = pathBytes)
        {
            var result = NativeMethods.tvg_saver_save_paint(Handle, paint.Handle, (sbyte*)pathPtr, quality);
            TvgResultHelper.CheckResult(result, "saver save paint");
        }
    }

    /// <summary>
    /// Saves an animation to a file.
    /// </summary>
    /// <param name="animation">Animation to save</param>
    /// <param name="path">File path</param>
    /// <param name="quality">Quality level (0-100, higher is better)</param>
    /// <param name="fps">Frames per second</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Save(TvgAnimation animation, string path, uint quality = 100, uint fps = 60)
    {
        ArgumentNullException.ThrowIfNull(animation);

        byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(path + '\0');
        fixed (byte* pathPtr = pathBytes)
        {
            var result = NativeMethods.tvg_saver_save_animation(Handle, animation.Handle, (sbyte*)pathPtr, quality, fps);
            TvgResultHelper.CheckResult(result, "saver save animation");
        }
    }

    /// <summary>
    /// Synchronizes the save operation (waits for completion).
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Sync()
    {
        var result = NativeMethods.tvg_saver_sync(Handle);
        TvgResultHelper.CheckResult(result, "saver sync");
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
                NativeMethods.tvg_saver_del(Handle);
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
    ~TvgSaver()
    {
        Dispose(false);
    }
}
