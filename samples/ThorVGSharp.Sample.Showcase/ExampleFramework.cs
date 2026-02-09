using Silk.NET.SDL;
using ThorVGSharp;

namespace ThorVGSharp.Sample.Showcase;

/// <summary>
/// Base class for ThorVG examples. Each example implements content() and optionally update(), clickdown(), clickup(), motion().
/// </summary>
internal abstract class Example : IDisposable
{
    public static string ResourceBasePath { get; set; } = "external/thorvg.example/res";

    public uint Elapsed { get; set; }

    /// <summary>
    /// Populate the canvas with initial content
    /// </summary>
    public abstract bool Content(TvgCanvas canvas, uint width, uint height);

    /// <summary>
    /// Update the canvas with animated content
    /// </summary>
    public virtual bool Update(TvgCanvas canvas, uint elapsed) => false;

    /// <summary>
    /// Handle mouse button down event
    /// </summary>
    public virtual bool ClickDown(TvgCanvas canvas, int x, int y) => false;

    /// <summary>
    /// Handle mouse button up event
    /// </summary>
    public virtual bool ClickUp(TvgCanvas canvas, int x, int y) => false;

    /// <summary>
    /// Handle mouse motion event
    /// </summary>
    public virtual bool Motion(TvgCanvas canvas, int x, int y) => false;

    /// <summary>
    /// Get current timestamp in seconds
    /// </summary>
    protected static float Timestamp() => Sdl.GetApi().GetTicks() * 0.001f;

    /// <summary>
    /// Load a resource file as byte array from the resource base path
    /// </summary>
    protected static byte[] LoadResource(string resourcePath)
    {
        var fullPath = Path.Combine(ResourceBasePath, resourcePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Resource file not found: {fullPath}");

        return File.ReadAllBytes(fullPath);
    }

    public virtual void Dispose() { }
}

/// <summary>
/// Calculates animation progress with optional rewind
/// </summary>
internal static class AnimationHelper
{
    public static float Progress(uint elapsed, float durationInSec, bool rewind = false)
    {
        var duration = (uint)(durationInSec * 1000.0f);
        if (elapsed == 0 || duration == 0) return 0.0f;

        var forward = ((elapsed / duration) % 2 == 0);
        if (elapsed % duration == 0) return forward ? 0.0f : 1.0f;

        var progress = (elapsed % duration) / (float)duration;
        if (rewind) return forward ? progress : (1 - progress);
        return progress;
    }
}
