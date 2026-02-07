using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Base class for gradient fills.
/// </summary>
public abstract class TvgFill : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the native handle to the gradient object.
    /// </summary>
    internal unsafe _Tvg_Gradient* Handle { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Fill class with the specified native handle.
    /// </summary>
    internal unsafe TvgFill(_Tvg_Gradient* handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Gets the unique value of the gradient instance indicating the instance type.
    /// </summary>
    /// <returns>The unique type of the gradient instance.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe TvgType GetGradientType()
    {
        Tvg_Type type;
        var result = NativeMethods.tvg_gradient_get_type(Handle, &type);
        TvgResultHelper.CheckResult(result, "get gradient type");
        return (TvgType)type;
    }

    /// <summary>
    /// Sets the color stops for the gradient.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public void SetColorStops(TvgColorStop[] stops)
    {
        if (stops == null || stops.Length == 0)
            throw new ArgumentException("Color stops cannot be null or empty.", nameof(stops));

        SetColorStops(stops.AsSpan());
    }

    /// <summary>
    /// Sets the color stops for the gradient.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetColorStops(ReadOnlySpan<TvgColorStop> stops)
    {
        if (stops.IsEmpty)
            throw new ArgumentException("Color stops cannot be empty.", nameof(stops));


        fixed (TvgColorStop* stopsPtr = stops)
        {
            var result = NativeMethods.tvg_gradient_set_color_stops(Handle, stopsPtr, (uint)stops.Length);
            TvgResultHelper.CheckResult(result, "gradient set color stops");
        }
    }

    /// <summary>
    /// Gets the color stops of the gradient.
    /// </summary>
    public unsafe TvgColorStop[] GetColorStops()
    {
        TvgColorStop* stopsPtr;
        uint count;
        var result = NativeMethods.tvg_gradient_get_color_stops(Handle, &stopsPtr, &count);

        if (result != Tvg_Result.TVG_RESULT_SUCCESS || stopsPtr == null || count == 0)
            return [];

        TvgColorStop[] stops = new TvgColorStop[count];
        for (int i = 0; i < count; i++)
            stops[i] = stopsPtr[i];

        return stops;
    }

    /// <summary>
    /// Sets the spread value, which specifies how to fill the area outside the gradient bounds.
    /// </summary>
    /// <param name="spread">The spread value.</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetSpread(TvgStrokeFill spread)
    {
        var result = NativeMethods.tvg_gradient_set_spread(Handle, (Tvg_Stroke_Fill)spread);
        TvgResultHelper.CheckResult(result, "set gradient spread");
    }

    /// <summary>
    /// Gets the spread value of the gradient object.
    /// </summary>
    /// <returns>The spread value.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe TvgStrokeFill GetSpread()
    {
        Tvg_Stroke_Fill spread;
        var result = NativeMethods.tvg_gradient_get_spread(Handle, &spread);
        TvgResultHelper.CheckResult(result, "get gradient spread");
        return (TvgStrokeFill)spread;
    }

    /// <summary>
    /// Sets the transformation matrix for the gradient.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTransform(TvgMatrix matrix)
    {
        var result = NativeMethods.tvg_gradient_set_transform(Handle, &matrix);
        TvgResultHelper.CheckResult(result, "gradient set transform");
    }

    /// <summary>
    /// Gets the transformation matrix of the gradient.
    /// </summary>
    public unsafe TvgMatrix GetTransform()
    {
        TvgMatrix matrix;
        NativeMethods.tvg_gradient_get_transform(Handle, &matrix);
        return matrix;
    }

    /// <summary>
    /// Creates a duplicate of this gradient.
    /// </summary>
    public unsafe TvgFill? Duplicate()
    {
        _Tvg_Gradient* duplicate = NativeMethods.tvg_gradient_duplicate(Handle);
        return CreateFromHandle(duplicate);
    }

    /// <summary>
    /// Creates a Fill wrapper from a native handle based on its type.
    /// </summary>
    internal static unsafe TvgFill? CreateFromHandle(_Tvg_Gradient* handle)
    {
        if (handle == null)
            return null;

        Tvg_Type type;
        NativeMethods.tvg_gradient_get_type(handle, &type);
        return type switch
        {
            Tvg_Type.TVG_TYPE_LINEAR_GRAD => new TvgLinearGradient(handle),
            Tvg_Type.TVG_TYPE_RADIAL_GRAD => new TvgRadialGradient(handle),
            _ => null
        };
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
                NativeMethods.tvg_gradient_del(Handle);
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
    ~TvgFill()
    {
        Dispose(false);
    }
}
