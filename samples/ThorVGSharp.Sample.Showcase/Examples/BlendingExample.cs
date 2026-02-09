using System.Runtime.InteropServices;
using ThorVGSharp;

namespace ThorVGSharp.Sample.Showcase.Examples;

/// <summary>
/// Blending Example - Demonstrates all ThorVG blend modes
/// Ported from: thorvg.example/src/Blending.cpp
/// </summary>
internal class BlendingExample : Example
{
    private uint[]? _imagePixels;
    private readonly List<TvgPaint> _paints = new();
    private readonly List<TvgFill> _fills = new();

    public override bool Content(TvgCanvas canvas, uint width, uint height)
    {
        // Load font
        var fontData = LoadResource("font/PublicSans-Regular.ttf");
        TvgFontManager.LoadData("PublicSans-Regular", fontData, "font/ttf");

        // Load raw image for blending demos
        var rawImageData = LoadResource("image/rawimage_200x300.raw");
        _imagePixels = new uint[200 * 300];
        Buffer.BlockCopy(rawImageData, 0, _imagePixels, 0, rawImageData.Length);

        // Create all blend mode examples
        CreateBlender(canvas, "Normal", TvgBlendMethod.Normal, 0.0f, 0.0f);
        CreateBlender(canvas, "Multiply", TvgBlendMethod.Multiply, 0.0f, 150.0f);
        CreateBlender(canvas, "Screen", TvgBlendMethod.Screen, 0.0f, 300.0f);
        CreateBlender(canvas, "Overlay", TvgBlendMethod.Overlay, 0.0f, 450.0f);
        CreateBlender(canvas, "Darken", TvgBlendMethod.Darken, 0.0f, 600.0f);
        CreateBlender(canvas, "Lighten", TvgBlendMethod.Lighten, 0.0f, 750.0f);
        CreateBlender(canvas, "ColorDodge", TvgBlendMethod.Colordodge, 0.0f, 900.0f);
        CreateBlender(canvas, "ColorBurn", TvgBlendMethod.Colorburn, 0.0f, 1050.0f);
        CreateBlender(canvas, "HardLight", TvgBlendMethod.Hardlight, 0.0f, 1200.0f);

        CreateBlender(canvas, "SoftLight", TvgBlendMethod.Softlight, 900.0f, 0.0f);
        CreateBlender(canvas, "Difference", TvgBlendMethod.Difference, 900.0f, 150.0f);
        CreateBlender(canvas, "Exclusion", TvgBlendMethod.Exclusion, 900.0f, 300.0f);
        CreateBlender(canvas, "Hue", TvgBlendMethod.Hue, 900.0f, 450.0f);
        CreateBlender(canvas, "Saturation", TvgBlendMethod.Saturation, 900.0f, 600.0f);
        CreateBlender(canvas, "Color", TvgBlendMethod.Color, 900.0f, 750.0f);
        CreateBlender(canvas, "Luminosity", TvgBlendMethod.Luminosity, 900.0f, 900.0f);
        CreateBlender(canvas, "Add", TvgBlendMethod.Add, 900.0f, 1050.0f);

        return true;
    }

    private void CreateBlender(TvgCanvas canvas, string name, TvgBlendMethod method, float x, float y)
    {
        // Title text
        var text = TvgText.Create();
        text.SetFont("PublicSans-Regular");
        text.SetSize(15);
        text.SetText(name);
        text.SetColor(255, 255, 255);
        text.Translate(x + 20, y);
        canvas.Add(text);
        _paints.Add(text);

        // Solid colors
        CreateSolidBlend(canvas, method, x + 20, y + 25, 255);
        CreateSolidBlend(canvas, method, x + 170, y + 25, 127);

        // Gradient blending
        CreateGradientBlend(canvas, method, x + 325, y + 25);

        // Image blending
        CreateImageBlend(canvas, method, x + 475, y + 25);

        // SVG blending
        CreateSvgBlend(canvas, method, x + 600, y + 25, 255);
        CreateSvgBlend(canvas, method, x + 750, y + 25, 127);
    }

    private void CreateSolidBlend(TvgCanvas canvas, TvgBlendMethod method, float x, float y, byte alpha)
    {
        var bottom = TvgShape.Create();
        bottom.AppendRect(x, y, 100, 100, 10, 10);
        bottom.SetFillColor(255, 255, 0, alpha);
        canvas.Add(bottom);
        _paints.Add(bottom);

        var top = TvgShape.Create();
        top.AppendRect(x + 25, y + 25, 100, 100, 10, 10);
        top.SetFillColor(0, 255, 255, alpha);
        top.SetBlendMethod(method);
        canvas.Add(top);
        _paints.Add(top);
    }

    private void CreateGradientBlend(TvgCanvas canvas, TvgBlendMethod method, float x, float y)
    {
        ReadOnlySpan<TvgColorStop> colorStops = stackalloc TvgColorStop[]
        {
            new(0, 255, 0, 255, 255),
            new(1, 0, 255, 0, 127)
        };

        var fill1 = TvgLinearGradient.Create();
        fill1.SetLinear(x, y, x + 100, y + 100);
        fill1.SetColorStops(colorStops);
        _fills.Add(fill1);

        var bottom = TvgShape.Create();
        bottom.AppendRect(x, y, 100, 100, 10, 10);
        bottom.SetFillGradient(fill1);
        canvas.Add(bottom);
        _paints.Add(bottom);

        var fill2 = TvgLinearGradient.Create();
        fill2.SetLinear(x + 25, y + 25, x + 125, y + 125);
        fill2.SetColorStops(colorStops);
        _fills.Add(fill2);

        var top = TvgShape.Create();
        top.AppendRect(x + 25, y + 25, 100, 100, 10, 10);
        top.SetFillGradient(fill2);
        top.SetBlendMethod(method);
        canvas.Add(top);
        _paints.Add(top);
    }

    private void CreateImageBlend(TvgCanvas canvas, TvgBlendMethod method, float x, float y)
    {
        if (_imagePixels == null) return;

        var bottom = TvgPicture.Create();
        bottom.LoadRaw(_imagePixels, 200, 300, TvgColorSpace.Argb8888, true);
        bottom.Translate(x, y);
        bottom.Scale(0.35f);
        canvas.Add(bottom);
        _paints.Add(bottom);

        var top = bottom.Duplicate() as TvgPicture;
        if (top != null)
        {
            top.Translate(x + 25, y + 25);
            top.Rotate(-10.0f);
            top.SetBlendMethod(method);
            canvas.Add(top);
            _paints.Add(top);
        }
    }

    private void CreateSvgBlend(TvgCanvas canvas, TvgBlendMethod method, float x, float y, byte opacity)
    {
        var svgData = LoadResource("svg/tiger.svg");
        var bottom = TvgPicture.Create();
        bottom.LoadData(svgData, "svg");
        bottom.Translate(x, y);
        bottom.Scale(0.11f);
        if (opacity < 255) bottom.SetOpacity(opacity);
        canvas.Add(bottom);
        _paints.Add(bottom);

        var top = bottom.Duplicate() as TvgPicture;
        if (top != null)
        {
            top.Translate(x + 25, y + 25);
            top.SetBlendMethod(method);
            canvas.Add(top);
            _paints.Add(top);
        }
    }

    public override void Dispose()
    {
        foreach (var paint in _paints)
            paint.Dispose();

        foreach (var fill in _fills)
            fill.Dispose();

        _paints.Clear();
        _fills.Clear();

        base.Dispose();
    }
}
