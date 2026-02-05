using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Represents a container for multiple paint objects.
/// </summary>
public class TvgScene : TvgPaint
{
    internal unsafe TvgScene(_Tvg_Paint* handle) : base(handle) { }

    /// <summary>
    /// Creates a new scene.
    /// </summary>
    public static unsafe TvgScene? Create()
    {
        _Tvg_Paint* handle = NativeMethods.tvg_scene_new();
        return handle != null ? new TvgScene(handle) : null;
    }

    /// <summary>
    /// Adds a paint to the scene.
    /// </summary>
    /// <remarks>
    /// The scene takes ownership of the paint object. However, this wrapper increments
    /// the reference count before adding, allowing C# to safely dispose the paint object
    /// while the scene also maintains a reference.
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Add(TvgPaint paint)
    {
        if (paint == null)
            throw new ArgumentNullException(nameof(paint));

        // Increment reference count to retain C# ownership while scene also owns it
        NativeMethods.tvg_paint_ref(paint.Handle);

        var result = NativeMethods.tvg_scene_add(Handle, paint.Handle);
        TvgResultHelper.CheckResult(result, "scene add paint");
    }

    /// <summary>
    /// Removes a paint from the scene.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Remove(TvgPaint paint)
    {
        if (paint == null)
            throw new ArgumentNullException(nameof(paint));

        var result = NativeMethods.tvg_scene_remove(Handle, paint.Handle);
        TvgResultHelper.CheckResult(result, "scene remove paint");
    }

    /// <summary>
    /// Inserts a target paint before the 'at' paint in the scene.
    /// </summary>
    /// <param name="target">Paint to insert</param>
    /// <param name="at">Paint to insert before (null to append at end)</param>
    /// <remarks>
    /// The scene takes ownership of the paint object. However, this wrapper increments
    /// the reference count before inserting, allowing C# to safely dispose the paint object
    /// while the scene also maintains a reference.
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Insert(TvgPaint target, TvgPaint? at = null)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        // Increment reference count to retain C# ownership while scene also owns it
        NativeMethods.tvg_paint_ref(target.Handle);

        var result = NativeMethods.tvg_scene_insert(Handle, target.Handle, at != null ? at.Handle : null);
        TvgResultHelper.CheckResult(result, "scene insert paint");
    }

    /// <summary>
    /// Clears all effects from the scene.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void ClearEffects()
    {
        var result = NativeMethods.tvg_scene_clear_effects(Handle);
        TvgResultHelper.CheckResult(result, "scene clear effects");
    }

    /// <summary>
    /// Adds a Gaussian blur effect to the scene.
    /// </summary>
    /// <param name="sigma">Blur intensity</param>
    /// <param name="direction">Blur direction (0 = both, 1 = horizontal, 2 = vertical)</param>
    /// <param name="border">Border handling (0 = clamp, 1 = wrap)</param>
    /// <param name="quality">Quality level (0-100)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AddGaussianBlurEffect(double sigma, int direction = 0, int border = 0, int quality = 100)
    {
        var result = NativeMethods.tvg_scene_add_effect_gaussian_blur(Handle, sigma, direction, border, quality);
        TvgResultHelper.CheckResult(result, "scene add gaussian blur effect");
    }

    /// <summary>
    /// Adds a drop shadow effect to the scene.
    /// </summary>
    /// <param name="r">Red component (0-255)</param>
    /// <param name="g">Green component (0-255)</param>
    /// <param name="b">Blue component (0-255)</param>
    /// <param name="a">Alpha component (0-255)</param>
    /// <param name="angle">Angle of the shadow in degrees</param>
    /// <param name="distance">Distance of the shadow</param>
    /// <param name="sigma">Blur intensity</param>
    /// <param name="quality">Quality level (0-100)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AddDropShadowEffect(int r, int g, int b, int a, double angle, double distance, double sigma, int quality = 100)
    {
        var result = NativeMethods.tvg_scene_add_effect_drop_shadow(Handle, r, g, b, a, angle, distance, sigma, quality);
        TvgResultHelper.CheckResult(result, "scene add drop shadow effect");
    }

    /// <summary>
    /// Adds a fill effect to the scene.
    /// </summary>
    /// <param name="r">Red component (0-255)</param>
    /// <param name="g">Green component (0-255)</param>
    /// <param name="b">Blue component (0-255)</param>
    /// <param name="a">Alpha component (0-255)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AddFillEffect(byte r, byte g, byte b, byte a = 255)
    {
        var result = NativeMethods.tvg_scene_add_effect_fill(Handle, r, g, b, a);
        TvgResultHelper.CheckResult(result, "scene add fill effect");
    }

    /// <summary>
    /// Adds a tint effect to the scene.
    /// </summary>
    /// <param name="blackR">Black color red component (0-255)</param>
    /// <param name="blackG">Black color green component (0-255)</param>
    /// <param name="blackB">Black color blue component (0-255)</param>
    /// <param name="whiteR">White color red component (0-255)</param>
    /// <param name="whiteG">White color green component (0-255)</param>
    /// <param name="whiteB">White color blue component (0-255)</param>
    /// <param name="intensity">Tint intensity (0.0 to 1.0)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AddTintEffect(int blackR, int blackG, int blackB, int whiteR, int whiteG, int whiteB, double intensity = 1.0)
    {
        var result = NativeMethods.tvg_scene_add_effect_tint(Handle, blackR, blackG, blackB, whiteR, whiteG, whiteB, intensity);
        TvgResultHelper.CheckResult(result, "scene add tint effect");
    }

    /// <summary>
    /// Adds a tritone effect to the scene.
    /// </summary>
    /// <param name="shadowR">Shadow color red component</param>
    /// <param name="shadowG">Shadow color green component</param>
    /// <param name="shadowB">Shadow color blue component</param>
    /// <param name="midtoneR">Midtone color red component</param>
    /// <param name="midtoneG">Midtone color green component</param>
    /// <param name="midtoneB">Midtone color blue component</param>
    /// <param name="highlightR">Highlight color red component</param>
    /// <param name="highlightG">Highlight color green component</param>
    /// <param name="highlightB">Highlight color blue component</param>
    /// <param name="blend">Blend mode</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void AddTritoneEffect(int shadowR, int shadowG, int shadowB,
        int midtoneR, int midtoneG, int midtoneB, int highlightR, int highlightG, int highlightB, int blend = 0)
    {
        var result = NativeMethods.tvg_scene_add_effect_tritone(Handle, shadowR, shadowG, shadowB, midtoneR, midtoneG, midtoneB, highlightR, highlightG, highlightB, blend);
        TvgResultHelper.CheckResult(result, "scene add tritone effect");
    }
}
