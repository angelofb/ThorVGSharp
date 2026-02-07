using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

using SkiaSharp;

using ThorVGSharp;

namespace ThorVGSharp.Benchmarks;

/// <summary>
/// Benchmark comparing complex scene rendering between ThorVGSharp and SkiaSharp.
/// Tests rendering performance with multiple shapes and transformations.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GradientsBenchmark
{
    private uint[]? _thorvgBuffer;
    private SKBitmap? _skiaBitmap;
    private const int Width = 1920;
    private const int Height = 1080;

    [GlobalSetup]
    public void Setup()
    {
        _thorvgBuffer = new uint[Width * Height];
        _skiaBitmap = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        TvgEngine.Initialize();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _skiaBitmap?.Dispose();
        TvgEngine.Terminate();
    }

    [Benchmark(Description = "Gradients - ThorVGSharp")]
    public void ThorVG_Gradients()
    {
        using var canvas = TvgCanvasSoftware.Create();
        canvas!.SetTarget(_thorvgBuffer!, (uint)Width, (uint)Width, (uint)Height, TvgColorSpace.Argb8888);

        for (int i = 0; i < 20; i++)
        {
            using var shape = TvgShape.Create();
            float x = (i % 5) * 360f + 50;
            float y = (i / 5) * 250f + 50;

            shape!.AppendRect(x, y, 300, 200, 0, 0);

            using var gradient = TvgLinearGradient.Create();
            gradient!.SetLinear(x, y, x + 300, y + 200);

            var colorStops = new TvgColorStop[]
            {
                new(0, 255, 100, 100, 255),
                new(0.5f, 100, 255, 100, 255),
                new(1, 100, 100, 255, 255)
            };
            gradient.SetColorStops(colorStops);

            shape.SetFillGradient(gradient);

            canvas.Add(shape);
        }

        canvas.Draw();
        canvas.Sync();
    }

    [Benchmark(Description = "Gradients - SkiaSharp")]
    public void SkiaSharp_Gradients()
    {
        using var surface = SKSurface.Create(_skiaBitmap!.Info, _skiaBitmap.GetPixels(), _skiaBitmap.RowBytes);
        using var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        for (int i = 0; i < 20; i++)
        {
            float x = (i % 5) * 360f + 50;
            float y = (i / 5) * 250f + 50;

            var colors = new[]
            {
                new SKColor(255, 100, 100, 255),
                new SKColor(100, 255, 100, 255),
                new SKColor(100, 100, 255, 255)
            };

            var positions = new[] { 0f, 0.5f, 1f };

            using var shader = SKShader.CreateLinearGradient(
                new SKPoint(x, y),
                new SKPoint(x + 300, y + 200),
                colors,
                positions,
                SKShaderTileMode.Clamp
            );

            using var paint = new SKPaint
            {
                Shader = shader,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawRect(x, y, 300, 200, paint);
        }

        surface.Flush();
    }
}
