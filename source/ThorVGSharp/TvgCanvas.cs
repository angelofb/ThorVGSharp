using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Abstract base class for all canvas types.
/// </summary>
public abstract class TvgCanvas : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the native handle to the canvas object.
    /// </summary>
    internal unsafe _Tvg_Canvas* Handle { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Canvas class with the specified native handle.
    /// </summary>
    internal unsafe TvgCanvas(_Tvg_Canvas* handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Adds a paint object to the canvas.
    /// </summary>
    /// <remarks>
    /// The canvas takes ownership of the paint object. However, this wrapper increments
    /// the reference count before adding, allowing C# to safely dispose the paint object
    /// while the canvas also maintains a reference.
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Add(TvgPaint paint)
    {
        ArgumentNullException.ThrowIfNull(paint);

        // Increment reference count to retain C# ownership while canvas also owns it
        NativeMethods.tvg_paint_ref(paint.Handle);

        var result = NativeMethods.tvg_canvas_add(Handle, paint.Handle);
        TvgResultHelper.CheckResult(result, "canvas add");
    }

    /// <summary>
    /// Removes a paint object from the canvas.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Remove(TvgPaint paint)
    {
        ArgumentNullException.ThrowIfNull(paint);

        var result = NativeMethods.tvg_canvas_remove(Handle, paint.Handle);
        TvgResultHelper.CheckResult(result, "canvas remove");
    }

    /// <summary>
    /// Inserts a target paint before the 'at' paint in the canvas.
    /// </summary>
    /// <param name="target">Paint to insert</param>
    /// <param name="at">Paint to insert before (null to append at end)</param>
    /// <remarks>
    /// The canvas takes ownership of the paint object. However, this wrapper increments
    /// the reference count before inserting, allowing C# to safely dispose the paint object
    /// while the canvas also maintains a reference.
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Insert(TvgPaint target, TvgPaint? at = null)
    {
        ArgumentNullException.ThrowIfNull(target);

        // Increment reference count to retain C# ownership while canvas also owns it
        NativeMethods.tvg_paint_ref(target.Handle);

        var result = NativeMethods.tvg_canvas_insert(Handle, target.Handle, at != null ? at.Handle : null);
        TvgResultHelper.CheckResult(result, "canvas insert");
    }

    /// <summary>
    /// Updates the canvas to prepare for rendering.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Update()
    {
        var result = NativeMethods.tvg_canvas_update(Handle);
        TvgResultHelper.CheckResult(result, "canvas update");
    }

    /// <summary>
    /// Draws the canvas contents.
    /// </summary>
    /// <param name="clear">Whether to clear the buffer before drawing</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Draw(bool clear = true)
    {
        var result = NativeMethods.tvg_canvas_draw(Handle, (byte)(clear ? 1 : 0));
        TvgResultHelper.CheckResult(result, "canvas draw");
    }

    /// <summary>
    /// Synchronizes rendering operations (waits for completion).
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Sync()
    {
        var result = NativeMethods.tvg_canvas_sync(Handle);
        TvgResultHelper.CheckResult(result, "canvas sync");
    }

    /// <summary>
    /// Sets the viewport for the canvas.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetViewport(int x, int y, int width, int height)
    {
        var result = NativeMethods.tvg_canvas_set_viewport(Handle, x, y, width, height);
        TvgResultHelper.CheckResult(result, "canvas set viewport");
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
                var result = NativeMethods.tvg_canvas_destroy(Handle);
                // Don't throw exceptions in Dispose, just ignore errors
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
    ~TvgCanvas()
    {
        Dispose(false);
    }
}
