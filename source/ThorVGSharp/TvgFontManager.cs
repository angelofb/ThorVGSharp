using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Manages font loading and unloading for text rendering.
/// </summary>
public static class TvgFontManager
{
    /// <summary>
    /// Loads a font from a file path.
    /// </summary>
    /// <param name="path">Path to the font file</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public static unsafe void Load(string path)
    {
        byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(path + '\0');
        fixed (byte* pathPtr = pathBytes)
        {
            var result = NativeMethods.tvg_font_load((sbyte*)pathPtr);
            TvgResultHelper.CheckResult(result, "font load");
        }
    }

    /// <summary>
    /// Loads a font from memory data.
    /// </summary>
    /// <param name="name">Font name identifier</param>
    /// <param name="data">Font file data span</param>
    /// <param name="mimeType">MIME type of the font (e.g., "font/ttf", "font/otf")</param>
    /// <param name="copy">Whether to copy the data</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public static unsafe void LoadData(string name, ReadOnlySpan<byte> data, string? mimeType = null, bool copy = true)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Font name cannot be null or empty.", nameof(name));

        if (data.IsEmpty)
            throw new ArgumentException("Font data cannot be empty.", nameof(data));

        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(name + '\0');
        byte[]? mimeBytes = mimeType != null ? System.Text.Encoding.UTF8.GetBytes(mimeType + '\0') : null;

        fixed (byte* namePtr = nameBytes)
        fixed (byte* dataPtr = data)
        fixed (byte* mimePtr = mimeBytes)
        {
            var result = NativeMethods.tvg_font_load_data((sbyte*)namePtr, (sbyte*)dataPtr, (uint)data.Length, (sbyte*)mimePtr, (byte)(copy ? 1 : 0));
            TvgResultHelper.CheckResult(result, "font load data");
        }
    }

    /// <summary>
    /// Unloads a previously loaded font.
    /// </summary>
    /// <param name="path">Path or name of the font to unload</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public static unsafe void Unload(string path)
    {
        byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(path + '\0');
        fixed (byte* pathPtr = pathBytes)
        {
            var result = NativeMethods.tvg_font_unload((sbyte*)pathPtr);
            TvgResultHelper.CheckResult(result, "font unload");
        }
    }
}
