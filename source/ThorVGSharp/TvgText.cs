using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Represents text rendering with font, styling, and layout capabilities.
/// </summary>
public class TvgText : TvgPaint
{
    internal unsafe TvgText(_Tvg_Paint* handle) : base(handle) { }

    /// <summary>
    /// Creates a new text object.
    /// </summary>
    public static unsafe TvgText? Create()
    {
        _Tvg_Paint* handle = NativeMethods.tvg_text_new();
        return handle != null ? new TvgText(handle) : null;
    }

    /// <summary>
    /// Sets the font for the text.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetFont(string fontName)
    {
        byte[] fontBytes = System.Text.Encoding.UTF8.GetBytes(fontName + '\0');
        fixed (byte* fontPtr = fontBytes)
        {
            var result = NativeMethods.tvg_text_set_font(Handle, (sbyte*)fontPtr);
            TvgResultHelper.CheckResult(result, "text set font");
        }
    }

    /// <summary>
    /// Sets the font size.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetSize(float size)
    {
        var result = NativeMethods.tvg_text_set_size(Handle, size);
        TvgResultHelper.CheckResult(result, "text set size");
    }

    /// <summary>
    /// Sets the text content (UTF-8).
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetText(string text)
    {
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + '\0');
        fixed (byte* textPtr = textBytes)
        {
            var result = NativeMethods.tvg_text_set_text(Handle, (sbyte*)textPtr);
            TvgResultHelper.CheckResult(result, "text set text");
        }
    }

    /// <summary>
    /// Aligns the text at the specified position.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Align(float x, float y)
    {
        var result = NativeMethods.tvg_text_align(Handle, x, y);
        TvgResultHelper.CheckResult(result, "text align");
    }

    /// <summary>
    /// Sets the layout constraints for the text.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void Layout(float width, float height)
    {
        var result = NativeMethods.tvg_text_layout(Handle, width, height);
        TvgResultHelper.CheckResult(result, "text layout");
    }

    /// <summary>
    /// Sets the text wrapping mode.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetWrapMode(TvgTextWrap wrapMode)
    {
        var result = NativeMethods.tvg_text_wrap_mode(Handle, (Tvg_Text_Wrap)wrapMode);
        TvgResultHelper.CheckResult(result, "text set wrap mode");
    }

    /// <summary>
    /// Sets letter and line spacing.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetSpacing(float letterSpacing, float lineSpacing)
    {
        var result = NativeMethods.tvg_text_spacing(Handle, letterSpacing, lineSpacing);
        TvgResultHelper.CheckResult(result, "text set spacing");
    }

    /// <summary>
    /// Sets italic style with the specified shear angle.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetItalic(float shear)
    {
        var result = NativeMethods.tvg_text_set_italic(Handle, shear);
        TvgResultHelper.CheckResult(result, "text set italic");
    }

    /// <summary>
    /// Sets the outline for the text.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetOutline(float width, byte r, byte g, byte b)
    {
        var result = NativeMethods.tvg_text_set_outline(Handle, width, r, g, b);
        TvgResultHelper.CheckResult(result, "text set outline");
    }

    /// <summary>
    /// Sets the text color.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetColor(byte r, byte g, byte b)
    {
        var result = NativeMethods.tvg_text_set_color(Handle, r, g, b);
        TvgResultHelper.CheckResult(result, "text set color");
    }

    /// <summary>
    /// Sets a gradient fill for the text.
    /// </summary>
    /// <exception cref="TvgException">Thrown when the operation fails.</exception>
    public unsafe void SetGradient(TvgFill? gradient)
    {
        var result = NativeMethods.tvg_text_set_gradient(Handle, gradient != null ? gradient.Handle : null);
        TvgResultHelper.CheckResult(result, "text set gradient");
    }
}
