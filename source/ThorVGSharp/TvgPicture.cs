using System.Runtime.InteropServices;

using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Delegate for resolving external assets in vector images (SVG, Lottie, etc.).
/// </summary>
/// <param name="paint">Target paint where the asset should be loaded</param>
/// <param name="src">Source path or identifier of the asset</param>
/// <returns>True if the asset was successfully resolved, false otherwise</returns>
public delegate bool TvgAssetResolver(TvgPaint paint, string src);

/// <summary>
/// Native callback delegate for asset resolution.
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate byte Tvg_Picture_Asset_Resolver(_Tvg_Paint* paint, sbyte* src, void* data);

/// <summary>
/// Represents a raster or vector image.
/// </summary>
public sealed class TvgPicture : TvgPaint
{
    // Keep reference to prevent GC
    private TvgAssetResolver? _assetResolver;
    private Tvg_Picture_Asset_Resolver? _nativeResolver;

    internal unsafe TvgPicture(_Tvg_Paint* handle) : base(handle) { }

    /// <summary>
    /// Creates a new picture.
    /// </summary>
    public static unsafe TvgPicture Create()
    {
        _Tvg_Paint* handle = NativeMethods.tvg_picture_new();
        return handle != null ? new TvgPicture(handle) : throw new TvgException("Failed to create picture.");
    }

    /// <summary>
    /// Loads an image from a file path.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Load(string path)
    {
        int maxBytes = StringHelper.GetMaxByteCount(path);
        Span<byte> buffer = maxBytes <= 256 ? stackalloc byte[maxBytes] : new byte[maxBytes];
        StringHelper.EncodeToUtf8(path, buffer);
        fixed (byte* pathPtr = buffer)
        {
            var result = NativeMethods.tvg_picture_load(Handle, (sbyte*)pathPtr);
            TvgResultHelper.CheckResult(result, "picture load");
        }
    }

    /// <summary>
    /// Loads a raw pixel buffer.
    /// </summary>
    /// <param name="data">Pixel data buffer span</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="colorSpace">Color space format</param>
    /// <param name="copy">Whether to copy the data (true) or reference it (false)</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void LoadRaw(ReadOnlySpan<uint> data, uint width, uint height, TvgColorSpace colorSpace, bool copy = true)
    {
        fixed (uint* dataPtr = data)
        {
            var result = NativeMethods.tvg_picture_load_raw(Handle, dataPtr, width, height, (Tvg_Colorspace)colorSpace, (byte)(copy ? 1 : 0));
            TvgResultHelper.CheckResult(result, "picture load raw");
        }
    }

    /// <summary>
    /// Loads image data from memory.
    /// </summary>
    /// <param name="data">Image data bytes span</param>
    /// <param name="mimeType">MIME type of the image (e.g., "image/png", "image/svg+xml")</param>
    /// <param name="resourcePath">Optional resource path for relative references</param>
    /// <param name="copy">Whether to copy the data</param>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void LoadData(ReadOnlySpan<byte> data, string? mimeType = null, string? resourcePath = null, bool copy = true)
    {
        int mimeMaxBytes = mimeType != null ? StringHelper.GetMaxByteCount(mimeType) : 0;
        int rpathMaxBytes = resourcePath != null ? StringHelper.GetMaxByteCount(resourcePath) : 0;

        Span<byte> mimeBuffer = mimeType != null
            ? (mimeMaxBytes <= 256 ? stackalloc byte[mimeMaxBytes] : new byte[mimeMaxBytes])
            : Span<byte>.Empty;
        Span<byte> rpathBuffer = resourcePath != null
            ? (rpathMaxBytes <= 256 ? stackalloc byte[rpathMaxBytes] : new byte[rpathMaxBytes])
            : Span<byte>.Empty;

        if (mimeType != null) StringHelper.EncodeToUtf8(mimeType, mimeBuffer);
        if (resourcePath != null) StringHelper.EncodeToUtf8(resourcePath, rpathBuffer);

        fixed (byte* dataPtr = data)
        fixed (byte* mimePtr = mimeBuffer)
        fixed (byte* rpathPtr = rpathBuffer)
        {
            var result = NativeMethods.tvg_picture_load_data(Handle, (sbyte*)dataPtr, (uint)data.Length,
                (sbyte*)mimePtr, (sbyte*)rpathPtr, (byte)(copy ? 1 : 0));
            TvgResultHelper.CheckResult(result, "picture load data");
        }
    }

    /// <summary>
    /// Sets the size of the picture.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetSize(float width, float height)
    {
        var result = NativeMethods.tvg_picture_set_size(Handle, width, height);
        TvgResultHelper.CheckResult(result, "picture set size");
    }

    /// <summary>
    /// Gets the size of the picture.
    /// </summary>
    public unsafe (float width, float height) GetSize()
    {
        float w, h;
        NativeMethods.tvg_picture_get_size(Handle, &w, &h);
        return (w, h);
    }

    /// <summary>
    /// Sets the origin point of the picture.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetOrigin(float x, float y)
    {
        var result = NativeMethods.tvg_picture_set_origin(Handle, x, y);
        TvgResultHelper.CheckResult(result, "picture set origin");
    }

    /// <summary>
    /// Gets the origin point of the picture.
    /// </summary>
    public unsafe (float x, float y) GetOrigin()
    {
        float x, y;
        NativeMethods.tvg_picture_get_origin(Handle, &x, &y);
        return (x, y);
    }

    /// <summary>
    /// Gets a child paint by ID (for SVG/vector images with nested elements).
    /// </summary>
    public unsafe TvgPaint? GetPaint(uint id)
    {
        _Tvg_Paint* paintHandle = NativeMethods.tvg_picture_get_paint(Handle, id);

        if (paintHandle == null)
            return null;

        return CreatePaintWrapper(paintHandle);
    }

    /// <summary>
    /// Sets a callback resolver for external assets in vector images.
    /// This must be called before Load().
    /// </summary>
    /// <param name="resolver">Callback to resolve asset paths, or null to unset</param>
    /// <remarks>
    /// The resolver is called when the picture needs to load external resources
    /// (images, fonts, etc.) referenced in the vector file.
    /// If the resolver returns false, ThorVG will attempt to resolve using its internal mechanism.
    /// </remarks>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetAssetResolver(TvgAssetResolver? resolver)
    {
        _assetResolver = resolver;

        if (resolver == null)
        {
            _nativeResolver = null;
            var result = NativeMethods.tvg_picture_set_asset_resolver(Handle, null, null);
            TvgResultHelper.CheckResult(result, "picture set asset resolver");
            return;
        }

        // Create native callback wrapper
        _nativeResolver = (paintPtr, srcPtr, userData) =>
        {
            try
            {
                var paint = CreatePaintWrapper(paintPtr);
                if (paint == null)
                    return 0;

                var src = Marshal.PtrToStringUTF8((nint)srcPtr) ?? string.Empty;
                return resolver(paint, src) ? (byte)1 : (byte)0;
            }
            catch
            {
                return 0;
            }
        };

        // Convert managed delegate to function pointer
        var funcPtr = Marshal.GetFunctionPointerForDelegate(_nativeResolver);
        var result2 = NativeMethods.tvg_picture_set_asset_resolver(Handle,
            (delegate* unmanaged[Cdecl]<_Tvg_Paint*, sbyte*, void*, byte>)funcPtr, null);
        TvgResultHelper.CheckResult(result2, "picture set asset resolver");
    }
}
