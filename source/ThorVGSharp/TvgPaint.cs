using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Base class for all drawable objects in ThorVG.
/// </summary>
public abstract class TvgPaint : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the native handle to the paint object.
    /// </summary>
    internal unsafe _Tvg_Paint* Handle { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Paint class with the specified native handle.
    /// </summary>
    internal unsafe TvgPaint(_Tvg_Paint* handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Gets the unique value of the paint instance indicating the instance type.
    /// </summary>
    /// <returns>The unique type of the paint instance.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe TvgType GetPaintType()
    {
        Tvg_Type type;
        var result = NativeMethods.tvg_paint_get_type(Handle, &type);
        TvgResultHelper.CheckResult(result, "get paint type");
        return (TvgType)type;
    }

    /// <summary>
    /// Sets the visibility of the Paint object.
    /// This is useful for selectively excluding paint objects during rendering.
    /// </summary>
    /// <param name="visible">A boolean flag indicating visibility. The default is true.
    /// If true, the object will be rendered by the engine.
    /// If false, the object will be excluded from the drawing process.</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    /// <remarks>
    /// An invisible object is not considered inactive—it may still participate
    /// in internal update processing if its properties are updated, but it will not
    /// be taken into account for the final drawing output. To completely deactivate
    /// a paint object, remove it from the canvas.
    /// </remarks>
    public unsafe void SetVisible(bool visible)
    {
        var result = NativeMethods.tvg_paint_set_visible(Handle, (byte)(visible ? 1 : 0));
        TvgResultHelper.CheckResult(result, "set paint visible");
    }

    /// <summary>
    /// Gets the current visibility status of the Paint object.
    /// </summary>
    /// <returns>True if the object is visible and will be rendered, false if the object is hidden and will not be rendered.</returns>
    public unsafe bool GetVisible()
    {
        return NativeMethods.tvg_paint_get_visible(Handle) != 0;
    }

    /// <summary>
    /// Sets the opacity of the paint.
    /// </summary>
    /// <param name="opacity">The opacity value in the range [0 ~ 255], where 0 is completely transparent and 255 is opaque.</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    /// <remarks>
    /// Setting the opacity with this API may require multiple renderings using a composition. It is recommended to avoid changing the opacity if possible.
    /// </remarks>
    public unsafe void SetOpacity(byte opacity)
    {
        var result = NativeMethods.tvg_paint_set_opacity(Handle, opacity);
        TvgResultHelper.CheckResult(result, "set paint opacity");
    }

    /// <summary>
    /// Gets the opacity of the paint.
    /// </summary>
    /// <returns>The opacity value in the range [0 ~ 255], where 0 is completely transparent and 255 is opaque.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe byte GetOpacity()
    {
        byte opacity;
        var result = NativeMethods.tvg_paint_get_opacity(Handle, &opacity);
        TvgResultHelper.CheckResult(result, "get paint opacity");
        return opacity;
    }

    /// <summary>
    /// Retrieves the current reference count of the Paint object.
    /// This method provides the current reference count, allowing the user to check the shared ownership state of the Paint object.
    /// </summary>
    /// <returns>The current reference count of the Paint object.</returns>
    public unsafe ushort GetRefCount()
    {
        return NativeMethods.tvg_paint_get_ref(Handle);
    }

    /// <summary>
    /// Scales the paint by the specified factor.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Scale(float factor)
    {
        var result = NativeMethods.tvg_paint_scale(Handle, factor);
        TvgResultHelper.CheckResult(result, "paint scale");
    }

    /// <summary>
    /// Rotates the paint by the specified angle in degrees.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Rotate(float degrees)
    {
        var result = NativeMethods.tvg_paint_rotate(Handle, degrees);
        TvgResultHelper.CheckResult(result, "paint rotate");
    }

    /// <summary>
    /// Translates the paint by the specified offset.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Translate(float x, float y)
    {
        var result = NativeMethods.tvg_paint_translate(Handle, x, y);
        TvgResultHelper.CheckResult(result, "paint translate");
    }

    /// <summary>
    /// Sets the transformation matrix for this paint.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTransform(TvgMatrix matrix)
    {
        var nativeMatrix = TvgMatrix.Map(matrix);
        var result = NativeMethods.tvg_paint_set_transform(Handle, &nativeMatrix);
        TvgResultHelper.CheckResult(result, "paint set transform");
    }

    /// <summary>
    /// Gets the transformation matrix of this paint.
    /// </summary>
    public unsafe TvgMatrix GetTransform()
    {
        Tvg_Matrix nativeMatrix;
        NativeMethods.tvg_paint_get_transform(Handle, &nativeMatrix);
        return TvgMatrix.Map(nativeMatrix);
    }

    /// <summary>
    /// Sets the blend method for this paint.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetBlendMethod(TvgBlendMethod method)
    {
        var result = NativeMethods.tvg_paint_set_blend_method(Handle, (Tvg_Blend_Method)method);
        TvgResultHelper.CheckResult(result, "paint set blend method");
    }

    /// <summary>
    /// Sets the mask method for this paint with a target paint.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetMaskMethod(TvgPaint target, TvgMaskMethod method)
    {
        ArgumentNullException.ThrowIfNull(target);

        var result = NativeMethods.tvg_paint_set_mask_method(Handle, target.Handle, (Tvg_Mask_Method)method);
        TvgResultHelper.CheckResult(result, "paint set mask method");
    }

    /// <summary>
    /// Gets the mask method for this paint with a target paint.
    /// </summary>
    public unsafe TvgMaskMethod GetMaskMethod(TvgPaint target)
    {
        ArgumentNullException.ThrowIfNull(target);

        Tvg_Mask_Method method;
        NativeMethods.tvg_paint_get_mask_method(Handle, target.Handle, &method);
        return (TvgMaskMethod)method;
    }

    /// <summary>
    /// Creates a duplicate of this paint.
    /// </summary>
    public unsafe TvgPaint? Duplicate()
    {
        _Tvg_Paint* duplicate = NativeMethods.tvg_paint_duplicate(Handle);
        if (duplicate == null)
            return null;

        return CreatePaintWrapper(duplicate);
    }

    /// <summary>
    /// Checks if this paint intersects with the specified rectangle.
    /// </summary>
    public unsafe bool Intersects(int x, int y, int width, int height)
    {
        byte result = NativeMethods.tvg_paint_intersects(Handle, x, y, width, height);
        return result != 0;
    }

    /// <summary>
    /// Gets the axis-aligned bounding box of this paint.
    /// </summary>
    public unsafe (float x, float y, float width, float height) GetBounds()
    {
        float x, y, w, h;
        NativeMethods.tvg_paint_get_aabb(Handle, &x, &y, &w, &h);
        return (x, y, w, h);
    }

    /// <summary>
    /// Gets the oriented bounding box (OBB) of this paint.
    /// Returns 4 corner points of the bounding box.
    /// </summary>
    /// <returns>Array of 4 points representing the corners of the OBB</returns>
    public unsafe (float x, float y)[] GetOrientedBounds()
    {
        var points = stackalloc Tvg_Point[4];
        var result = NativeMethods.tvg_paint_get_obb(Handle, points);

        if (result != Tvg_Result.TVG_RESULT_SUCCESS)
            return Array.Empty<(float, float)>();

        var output = new (float x, float y)[4];
        for (int i = 0; i < 4; i++)
        {
            output[i] = (points[i].x, points[i].y);
        }
        return output;
    }

    /// <summary>
    /// Gets the parent paint object if this paint is part of a scene.
    /// </summary>
    /// <returns>The parent paint, or null if no parent</returns>
    public unsafe TvgPaint? GetParent()
    {
        _Tvg_Paint* parent = NativeMethods.tvg_paint_get_parent(Handle);
        return CreatePaintWrapper(parent);
    }

    /// <summary>
    /// Sets a clip paint for masking this paint.
    /// </summary>
    /// <param name="clipper">The paint object to use as a clipper (typically a Shape)</param>
    /// <remarks>
    /// The clipper defines the visible region of this paint.
    /// Only Shape objects are supported as clippers.
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetClip(TvgPaint clipper)
    {
        ArgumentNullException.ThrowIfNull(clipper);

        var result = NativeMethods.tvg_paint_set_clip(Handle, clipper.Handle);
        TvgResultHelper.CheckResult(result, "paint set clip");
    }

    /// <summary>
    /// Gets the clip paint object that was set for this paint.
    /// </summary>
    /// <returns>The clip paint, or null if no clip is set</returns>
    public unsafe TvgPaint? GetClip()
    {
        _Tvg_Paint* clip = NativeMethods.tvg_paint_get_clip(Handle);
        return CreatePaintWrapper(clip);
    }

    /// <summary>
    /// Releases this paint object immediately.
    /// </summary>
    /// <remarks>
    /// This is a direct release method, alternative to Dispose().
    /// Use this only when you need immediate release without reference counting.
    /// For normal usage, prefer using 'using' statements which call Dispose().
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Release()
    {
        if (Handle == null)
            return;

        var result = NativeMethods.tvg_paint_rel(Handle);
        TvgResultHelper.CheckResult(result, "paint release");
        Handle = null;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Increments the reference count of this paint.
    /// </summary>
    /// <returns>The new reference count</returns>
    public unsafe ushort Ref()
    {
        return NativeMethods.tvg_paint_ref(Handle);
    }

    /// <summary>
    /// Decrements the reference count and optionally frees the paint.
    /// </summary>
    /// <returns>The new reference count</returns>
    public unsafe ushort Unref(bool free = false)
    {
        return NativeMethods.tvg_paint_unref(Handle, (byte)(free ? 1 : 0));
    }

    /// <summary>
    /// Creates a wrapper for a paint handle based on its type.
    /// </summary>
    internal static unsafe TvgPaint? CreatePaintWrapper(_Tvg_Paint* handle)
    {
        if (handle == null)
            return null;

        Tvg_Type type;
        NativeMethods.tvg_paint_get_type(handle, &type);
        return type switch
        {
            Tvg_Type.TVG_TYPE_SHAPE => new TvgShape(handle),
            Tvg_Type.TVG_TYPE_SCENE => new TvgScene(handle),
            Tvg_Type.TVG_TYPE_PICTURE => new TvgPicture(handle),
            Tvg_Type.TVG_TYPE_TEXT => new TvgText(handle),
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
                // Use unref with free=true to properly handle reference counting
                // This allows the paint to be shared between C# and native canvas/scene
                NativeMethods.tvg_paint_unref(Handle, 1);
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
    ~TvgPaint()
    {
        Dispose(false);
    }
}
