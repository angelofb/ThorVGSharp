using System.Runtime.InteropServices;

using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Provides functionality to traverse scene graphs and access nested paint objects.
/// </summary>
public class TvgAccessor : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the native handle to the accessor object.
    /// </summary>
    internal unsafe _Tvg_Accessor* Handle { get; private set; }

    internal unsafe TvgAccessor(_Tvg_Accessor* handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Creates a new accessor.
    /// </summary>
    public static unsafe TvgAccessor Create()
    {
        _Tvg_Accessor* handle = NativeMethods.tvg_accessor_new();
        return handle != null ? new TvgAccessor(handle) : throw new TvgException("Failed to create accessor.");
    }

    /// <summary>
    /// Delegate for paint visitor callback.
    /// </summary>
    /// <param name="paint">The paint object being visited</param>
    /// <param name="userData">User data passed to the traversal</param>
    /// <returns>True to continue traversal, false to stop</returns>
    public delegate bool PaintVisitor(TvgPaint paint, IntPtr userData);

    /// <summary>
    /// Sets a visitor callback for scene graph traversal on a paint object.
    /// </summary>
    /// <param name="paint">Paint object to traverse</param>
    /// <param name="visitor">Visitor callback function</param>
    /// <param name="userData">User data to pass to the callback</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Set(TvgPaint paint, PaintVisitor visitor, IntPtr userData = default)
    {
        ArgumentNullException.ThrowIfNull(paint);

        // Create a native callback wrapper
        delegate* unmanaged[Cdecl]<_Tvg_Paint*, void*, byte> nativeCallback = &NativeVisitorCallback;

        // Store the managed callback in a GCHandle to prevent collection
        var callbackHandle = GCHandle.Alloc(visitor);

        try
        {
            var result = NativeMethods.tvg_accessor_set(Handle, paint.Handle, nativeCallback, GCHandle.ToIntPtr(callbackHandle).ToPointer());
            TvgResultHelper.CheckResult(result, "accessor set");
        }
        catch
        {
            callbackHandle.Free();
            throw;
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static unsafe byte NativeVisitorCallback(_Tvg_Paint* paint, void* userData)
    {
        try
        {
            var callbackHandle = GCHandle.FromIntPtr(new IntPtr(userData));
            var visitor = (PaintVisitor)callbackHandle.Target!;

            var managedPaint = TvgPaint.CreatePaintWrapper(paint);
            if (managedPaint == null)
                return 0;

            return (byte)(visitor(managedPaint, IntPtr.Zero) ? 1 : 0);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Generates a unique ID from a name string.
    /// </summary>
    public static unsafe uint GenerateId(string name)
    {
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(name + '\0');
        fixed (byte* namePtr = nameBytes)
        {
            return NativeMethods.tvg_accessor_generate_id((sbyte*)namePtr);
        }
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
                NativeMethods.tvg_accessor_del(Handle);
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
    ~TvgAccessor()
    {
        Dispose(false);
    }
}
