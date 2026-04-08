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
        int maxBytes = StringHelper.GetMaxByteCount(path);
        Span<byte> buffer = maxBytes <= 256 ? stackalloc byte[maxBytes] : new byte[maxBytes];
        StringHelper.EncodeToUtf8(path, buffer);
        fixed (byte* pathPtr = buffer)
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

        if (!copy)
            throw new ArgumentException("copy=false is unsafe with ReadOnlySpan input. Use LoadData(string, IntPtr, uint, ...) for unmanaged or pinned buffers.", nameof(copy));

        fixed (byte* dataPtr = data)
        {
            LoadData(name, (IntPtr)dataPtr, (uint)data.Length, mimeType, copy: true);
        }
    }

    /// <summary>
    /// Loads a font from unmanaged or externally pinned memory data.
    /// </summary>
    /// <param name="name">Font name identifier</param>
    /// <param name="data">Pointer to font data</param>
    /// <param name="size">Font data size in bytes</param>
    /// <param name="mimeType">MIME type of the font (e.g., "font/ttf", "font/otf")</param>
    /// <param name="copy">Whether to copy the data</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public static unsafe void LoadData(string name, IntPtr data, uint size, string? mimeType = null, bool copy = false)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Font name cannot be null or empty.", nameof(name));

        if (data == IntPtr.Zero && size > 0)
            throw new ArgumentException("Data pointer cannot be null when size is greater than zero.", nameof(data));

        int nameMaxBytes = StringHelper.GetMaxByteCount(name);
        int mimeMaxBytes = mimeType != null ? StringHelper.GetMaxByteCount(mimeType) : 0;

        Span<byte> nameBuffer = nameMaxBytes <= 256 ? stackalloc byte[nameMaxBytes] : new byte[nameMaxBytes];
        StringHelper.EncodeToUtf8(name, nameBuffer);

        Span<byte> mimeBuffer = mimeType != null
            ? (mimeMaxBytes <= 256 ? stackalloc byte[mimeMaxBytes] : new byte[mimeMaxBytes])
            : [];
        if (mimeType != null) StringHelper.EncodeToUtf8(mimeType, mimeBuffer);

        fixed (byte* namePtr = nameBuffer)
        fixed (byte* mimePtr = mimeBuffer)
        {
            var result = NativeMethods.tvg_font_load_data((sbyte*)namePtr, (sbyte*)data, size, (sbyte*)mimePtr, (byte)(copy ? 1 : 0));
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
        int maxBytes = StringHelper.GetMaxByteCount(path);
        Span<byte> buffer = maxBytes <= 256 ? stackalloc byte[maxBytes] : new byte[maxBytes];
        StringHelper.EncodeToUtf8(path, buffer);
        fixed (byte* pathPtr = buffer)
        {
            var result = NativeMethods.tvg_font_unload((sbyte*)pathPtr);
            TvgResultHelper.CheckResult(result, "font unload");
        }
    }
}
