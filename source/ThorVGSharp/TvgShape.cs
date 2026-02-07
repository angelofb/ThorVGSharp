using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Represents a vector shape with path data, fill, and stroke properties.
/// </summary>
public sealed class TvgShape : TvgPaint
{
    internal unsafe TvgShape(_Tvg_Paint* handle) : base(handle) { }

    /// <summary>
    /// Creates a new shape.
    /// </summary>
    public static unsafe TvgShape Create()
    {
        _Tvg_Paint* handle = NativeMethods.tvg_shape_new();
        return handle != null ? new TvgShape(handle) : throw new TvgException("Failed to create shape.");
    }

    /// <summary>
    /// Resets the shape, clearing all path data.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Reset()
    {
        var result = NativeMethods.tvg_shape_reset(Handle);
        TvgResultHelper.CheckResult(result, "shape reset");
    }

    #region Path Operations

    /// <summary>
    /// Moves the current point to the specified position.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void MoveTo(float x, float y)
    {
        var result = NativeMethods.tvg_shape_move_to(Handle, x, y);
        TvgResultHelper.CheckResult(result, "shape move to");
    }

    /// <summary>
    /// Adds a line from the current point to the specified position.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void LineTo(float x, float y)
    {
        var result = NativeMethods.tvg_shape_line_to(Handle, x, y);
        TvgResultHelper.CheckResult(result, "shape line to");
    }

    /// <summary>
    /// Adds a cubic Bézier curve to the path.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void CubicTo(float cx1, float cy1, float cx2, float cy2, float x, float y)
    {
        var result = NativeMethods.tvg_shape_cubic_to(Handle, cx1, cy1, cx2, cy2, x, y);
        TvgResultHelper.CheckResult(result, "shape cubic to");
    }

    /// <summary>
    /// Closes the current path.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Close()
    {
        var result = NativeMethods.tvg_shape_close(Handle);
        TvgResultHelper.CheckResult(result, "shape close");
    }

    /// <summary>
    /// Appends a custom path defined by commands and points.
    /// </summary>
    /// <param name="commands">Span of path commands</param>
    /// <param name="points">Span of points corresponding to the commands</param>
    /// <remarks>
    /// This allows appending complex paths defined programmatically or loaded from external sources.
    /// Each command requires a specific number of points:
    /// - MoveTo: 1 point (x, y)
    /// - LineTo: 1 point (x, y)
    /// - CubicTo: 3 points (cx1, cy1, cx2, cy2, x, y)
    /// - Close: 0 points
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AppendPath(ReadOnlySpan<TvgPathCommand> commands, ReadOnlySpan<(float x, float y)> points)
    {
        if (commands.IsEmpty)
            throw new ArgumentException("Commands span cannot be empty", nameof(commands));

        // Convert commands to byte array or stackalloc for small sizes
        Span<byte> cmdBytes = commands.Length <= 256
            ? stackalloc byte[commands.Length]
            : new byte[commands.Length];

        for (int i = 0; i < commands.Length; i++)
            cmdBytes[i] = (byte)commands[i];

        // Convert points to native format
        Span<Tvg_Point> nativePoints = points.Length <= 256
            ? stackalloc Tvg_Point[points.Length]
            : new Tvg_Point[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            nativePoints[i].x = points[i].x;
            nativePoints[i].y = points[i].y;
        }

        fixed (byte* cmdsPtr = cmdBytes)
        fixed (Tvg_Point* pointsPtr = nativePoints)
        {
            var result = NativeMethods.tvg_shape_append_path(Handle, cmdsPtr, (uint)commands.Length, pointsPtr, (uint)points.Length);
            TvgResultHelper.CheckResult(result, "shape append path");
        }
    }

    /// <summary>
    /// Appends a rectangle to the shape.
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Width of the rectangle</param>
    /// <param name="height">Height of the rectangle</param>
    /// <param name="rx">X-axis radius for rounded corners</param>
    /// <param name="ry">Y-axis radius for rounded corners</param>
    /// <param name="clockwise">Path direction (true for clockwise)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AppendRect(float x, float y, float width, float height, float rx = 0, float ry = 0, bool clockwise = true)
    {
        var result = NativeMethods.tvg_shape_append_rect(Handle, x, y, width, height, rx, ry, (byte)(clockwise ? 1 : 0));
        TvgResultHelper.CheckResult(result, "shape append rect");
    }

    /// <summary>
    /// Appends a circle (or ellipse) to the shape.
    /// </summary>
    /// <param name="cx">Center X coordinate</param>
    /// <param name="cy">Center Y coordinate</param>
    /// <param name="rx">X-axis radius</param>
    /// <param name="ry">Y-axis radius</param>
    /// <param name="clockwise">Path direction (true for clockwise)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AppendCircle(float cx, float cy, float rx, float ry, bool clockwise = true)
    {
        var result = NativeMethods.tvg_shape_append_circle(Handle, cx, cy, rx, ry, (byte)(clockwise ? 1 : 0));
        TvgResultHelper.CheckResult(result, "shape append circle");
    }

    /// <summary>
    /// Appends a circle to the shape.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public void AppendCircle(float cx, float cy, float radius, bool clockwise = true)
    {
        AppendCircle(cx, cy, radius, radius, clockwise);
    }

    #endregion

    #region Fill Properties

    /// <summary>
    /// Sets the fill color of the shape.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetFillColor(byte r, byte g, byte b, byte a = 255)
    {
        var result = NativeMethods.tvg_shape_set_fill_color(Handle, r, g, b, a);
        TvgResultHelper.CheckResult(result, "shape set fill color");
    }

    /// <summary>
    /// Gets the fill color of the shape.
    /// </summary>
    public unsafe (byte r, byte g, byte b, byte a) GetFillColor()
    {
        byte r, g, b, a;
        NativeMethods.tvg_shape_get_fill_color(Handle, &r, &g, &b, &a);
        return (r, g, b, a);
    }

    /// <summary>
    /// Sets the fill rule for the shape.
    /// Specifies how the interior of the shape is determined when its path intersects itself.
    /// The default fill rule is NonZero.
    /// </summary>
    /// <param name="rule">The fill rule to apply to the shape.</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetFillRule(TvgFillRule rule)
    {
        var result = NativeMethods.tvg_shape_set_fill_rule(Handle, (Tvg_Fill_Rule)rule);
        TvgResultHelper.CheckResult(result, "set fill rule");
    }

    /// <summary>
    /// Retrieves the current fill rule used by the shape.
    /// This function returns the fill rule, which determines how the interior
    /// regions of the shape are calculated when it overlaps itself.
    /// </summary>
    /// <returns>The current fill rule value of the shape.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe TvgFillRule GetFillRule()
    {
        Tvg_Fill_Rule rule;
        var result = NativeMethods.tvg_shape_get_fill_rule(Handle, &rule);
        TvgResultHelper.CheckResult(result, "get fill rule");
        return (TvgFillRule)rule;
    }

    /// <summary>
    /// Sets a gradient fill for the shape.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetFillGradient(TvgFill? gradient)
    {
        var result = NativeMethods.tvg_shape_set_gradient(Handle, gradient != null ? gradient.Handle : null);
        TvgResultHelper.CheckResult(result, "shape set fill gradient");
    }

    /// <summary>
    /// Gets the fill gradient of the shape.
    /// </summary>
    public unsafe TvgFill? GetFillGradient()
    {
        _Tvg_Gradient* gradientHandle;
        NativeMethods.tvg_shape_get_gradient(Handle, &gradientHandle);
        return TvgFill.CreateFromHandle(gradientHandle);
    }

    #endregion

    #region Stroke Properties

    /// <summary>
    /// Sets the stroke width for the path.
    /// This function defines the thickness of the stroke applied to all figures
    /// in the path object. A stroke is the outline drawn along the edges of the
    /// path's geometry.
    /// </summary>
    /// <param name="width">The width of the stroke in pixels. Must be positive value. (The default is 0)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    /// <remarks>
    /// A value of 0 disables the stroke.
    /// </remarks>
    public unsafe void SetStrokeWidth(float width)
    {
        var result = NativeMethods.tvg_shape_set_stroke_width(Handle, width);
        TvgResultHelper.CheckResult(result, "set stroke width");
    }

    /// <summary>
    /// Gets the shape's stroke width.
    /// </summary>
    /// <returns>The stroke width.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe float GetStrokeWidth()
    {
        float width;
        var result = NativeMethods.tvg_shape_get_stroke_width(Handle, &width);
        TvgResultHelper.CheckResult(result, "get stroke width");
        return width;
    }

    /// <summary>
    /// Sets the stroke color.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetStrokeColor(byte r, byte g, byte b, byte a = 255)
    {
        var result = NativeMethods.tvg_shape_set_stroke_color(Handle, r, g, b, a);
        TvgResultHelper.CheckResult(result, "shape set stroke color");
    }

    /// <summary>
    /// Gets the stroke color.
    /// </summary>
    public unsafe (byte r, byte g, byte b, byte a) GetStrokeColor()
    {
        byte r, g, b, a;
        NativeMethods.tvg_shape_get_stroke_color(Handle, &r, &g, &b, &a);
        return (r, g, b, a);
    }

    /// <summary>
    /// Sets a gradient stroke for the shape.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetStrokeGradient(TvgFill? gradient)
    {
        var result = NativeMethods.tvg_shape_set_stroke_gradient(Handle, gradient != null ? gradient.Handle : null);
        TvgResultHelper.CheckResult(result, "shape set stroke gradient");
    }

    /// <summary>
    /// Gets the stroke gradient.
    /// </summary>
    public unsafe TvgFill? GetStrokeGradient()
    {
        _Tvg_Gradient* gradientHandle;
        NativeMethods.tvg_shape_get_stroke_gradient(Handle, &gradientHandle);
        return TvgFill.CreateFromHandle(gradientHandle);
    }

    /// <summary>
    /// Sets the cap style used for stroking the path.
    /// The cap style specifies the shape to be used at the end of the open stroked sub-paths.
    /// </summary>
    /// <param name="cap">The cap style value. The default value is Square.</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetStrokeCap(TvgStrokeCap cap)
    {
        var result = NativeMethods.tvg_shape_set_stroke_cap(Handle, (Tvg_Stroke_Cap)cap);
        TvgResultHelper.CheckResult(result, "set stroke cap");
    }

    /// <summary>
    /// Gets the stroke cap style used for stroking the path.
    /// </summary>
    /// <returns>The cap style value.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe TvgStrokeCap GetStrokeCap()
    {
        Tvg_Stroke_Cap cap;
        var result = NativeMethods.tvg_shape_get_stroke_cap(Handle, &cap);
        TvgResultHelper.CheckResult(result, "get stroke cap");
        return (TvgStrokeCap)cap;
    }

    /// <summary>
    /// Sets the join style for stroked path segments.
    /// </summary>
    /// <param name="join">The join style value. The default value is Bevel.</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetStrokeJoin(TvgStrokeJoin join)
    {
        var result = NativeMethods.tvg_shape_set_stroke_join(Handle, (Tvg_Stroke_Join)join);
        TvgResultHelper.CheckResult(result, "set stroke join");
    }

    /// <summary>
    /// Gets the stroke join method.
    /// </summary>
    /// <returns>The join style value.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe TvgStrokeJoin GetStrokeJoin()
    {
        Tvg_Stroke_Join join;
        var result = NativeMethods.tvg_shape_get_stroke_join(Handle, &join);
        TvgResultHelper.CheckResult(result, "get stroke join");
        return (TvgStrokeJoin)join;
    }

    /// <summary>
    /// Sets the stroke miterlimit.
    /// </summary>
    /// <param name="miterlimit">The miterlimit imposes a limit on the extent of the stroke join when the Miter join style is set. The default value is 4.</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    /// <remarks>
    /// Unsupported miterlimit values (less than zero) will cause an error.
    /// </remarks>
    public unsafe void SetStrokeMiterLimit(float miterlimit)
    {
        var result = NativeMethods.tvg_shape_set_stroke_miterlimit(Handle, miterlimit);
        TvgResultHelper.CheckResult(result, "set stroke miter limit");
    }

    /// <summary>
    /// Gets the stroke miterlimit.
    /// </summary>
    /// <returns>The stroke miterlimit.</returns>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe float GetStrokeMiterLimit()
    {
        float limit;
        var result = NativeMethods.tvg_shape_get_stroke_miterlimit(Handle, &limit);
        TvgResultHelper.CheckResult(result, "get stroke miter limit");
        return limit;
    }

    /// <summary>
    /// Sets the stroke dash pattern.
    /// </summary>
    /// <param name="dashPattern">Span of dash pattern values. Pass an empty span to clear the dash pattern.</param>
    /// <param name="offset">Offset for the dash pattern</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetStrokeDash(ReadOnlySpan<float> dashPattern, float offset = 0)
    {
        if (dashPattern.IsEmpty)
        {
            var result = NativeMethods.tvg_shape_set_stroke_dash(Handle, null, 0, 0);
            TvgResultHelper.CheckResult(result, "shape set stroke dash");
            return;
        }

        fixed (float* dashPtr = dashPattern)
        {
            var result = NativeMethods.tvg_shape_set_stroke_dash(Handle, dashPtr, (uint)dashPattern.Length, offset);
            TvgResultHelper.CheckResult(result, "shape set stroke dash");
        }
    }

    /// <summary>
    /// Gets the stroke dash pattern.
    /// </summary>
    public unsafe (float[]? pattern, float offset) GetStrokeDash()
    {
        float* dashPtr;
        uint count;
        float offset;
        var result = NativeMethods.tvg_shape_get_stroke_dash(Handle, &dashPtr, &count, &offset);

        if (result != Tvg_Result.TVG_RESULT_SUCCESS || dashPtr == null || count == 0)
            return (null, 0);

        float[] pattern = new float[count];
        for (int i = 0; i < count; i++)
            pattern[i] = dashPtr[i];

        return (pattern, offset);
    }

    #endregion

    #region Advanced Features

    /// <summary>
    /// Sets whether stroke should be painted before fill.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetPaintOrder(bool strokeFirst)
    {
        var result = NativeMethods.tvg_shape_set_paint_order(Handle, (byte)(strokeFirst ? 1 : 0));
        TvgResultHelper.CheckResult(result, "shape set paint order");
    }

    /// <summary>
    /// Trims the path for animation effects.
    /// </summary>
    /// <param name="begin">Start position (0.0 to 1.0)</param>
    /// <param name="end">End position (0.0 to 1.0)</param>
    /// <param name="simultaneous">Whether to trim simultaneously</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetTrimPath(float begin, float end, bool simultaneous = true)
    {
        var result = NativeMethods.tvg_shape_set_trimpath(Handle, begin, end, (byte)(simultaneous ? 1 : 0));
        TvgResultHelper.CheckResult(result, "shape set trim path");
    }

    /// <summary>
    /// Gets the path commands and points that define this shape.
    /// </summary>
    /// <returns>Tuple containing (commands array, points array)</returns>
    public unsafe (TvgPathCommand[] commands, (float x, float y)[] points) GetPath()
    {
        byte* commandsPtr;
        uint commandCount;
        Tvg_Point* pointsPtr;
        uint pointCount;

        var result = NativeMethods.tvg_shape_get_path(Handle, &commandsPtr, &commandCount, &pointsPtr, &pointCount);
        if (result != Tvg_Result.TVG_RESULT_SUCCESS)
            return (Array.Empty<TvgPathCommand>(), Array.Empty<(float, float)>());

        var commands = new TvgPathCommand[commandCount];
        for (int i = 0; i < commandCount; i++)
            commands[i] = (TvgPathCommand)commandsPtr[i];

        var points = new (float x, float y)[pointCount];
        for (int i = 0; i < pointCount; i++)
            points[i] = (pointsPtr[i].x, pointsPtr[i].y);

        return (commands, points);
    }

    #endregion
}
