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
public class ComplexSceneBenchmark
{
    private uint[]? _thorvgBuffer;
    private SKBitmap? _skiaBitmap;
    private const int Width = 1920;
    private const int Height = 1080;
    private const int ShapeCount = 100;

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

    [Benchmark(Description = "ThorVG - 100 Shapes")]
    public void ThorVG_MultipleShapes()
    {
        using var canvas = TvgCanvasSoftware.Create();
        canvas!.SetTarget(_thorvgBuffer!, (uint)Width, (uint)Width, (uint)Height, TvgColorSpace.Argb8888);

        using var scene = TvgScene.Create();

        for (int i = 0; i < ShapeCount; i++)
        {
            using var shape = TvgShape.Create();
            float x = (i % 10) * 180f + 50;
            float y = (i / 10) * 100f + 50;

            shape!.AppendRect(x, y, 150, 80, 10, 10);
            shape.SetFillColor(
                (byte)((i * 17) % 256),
                (byte)((i * 31) % 256),
                (byte)((i * 47) % 256),
                255
            );

            scene!.Add(shape);
        }

        canvas.Add(scene!);
        canvas.Draw();
        canvas.Sync();
    }

    [Benchmark(Description = "SkiaSharp - 100 Shapes")]
    public void SkiaSharp_MultipleShapes()
    {
        using var surface = SKSurface.Create(_skiaBitmap!.Info, _skiaBitmap.GetPixels(), _skiaBitmap.RowBytes);
        using var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        for (int i = 0; i < ShapeCount; i++)
        {
            float x = (i % 10) * 180f + 50;
            float y = (i / 10) * 100f + 50;

            using var paint = new SKPaint
            {
                Color = new SKColor(
                    (byte)((i * 17) % 256),
                    (byte)((i * 31) % 256),
                    (byte)((i * 47) % 256),
                    255
                ),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            using var roundRect = new SKRoundRect(new SKRect(x, y, x + 150, y + 80), 10, 10);
            canvas.DrawRoundRect(roundRect, paint);
        }

        surface.Flush();
    }

    [Benchmark(Description = "ThorVG - Gradients")]
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
                new TvgColorStop { Offset = 0, R = 255, G = 100, B = 100, A = 255 },
                new TvgColorStop { Offset = 0.5f, R = 100, G = 255, B = 100, A = 255 },
                new TvgColorStop { Offset = 1, R = 100, G = 100, B = 255, A = 255 }
            };
            gradient.SetColorStops(colorStops);

            shape.SetFillGradient(gradient);

            canvas.Add(shape);
        }

        canvas.Draw();
        canvas.Sync();
    }

    [Benchmark(Description = "SkiaSharp - Gradients")]
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
