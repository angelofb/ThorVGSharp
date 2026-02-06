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

    [Params(50, 500, 5000)]
    public int ShapeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _thorvgBuffer = new uint[Width * Height];
        _skiaBitmap = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        TvgEngine.Initialize(4);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _skiaBitmap?.Dispose();
        TvgEngine.Terminate();
    }

    [Benchmark(Description = "Multiple Shapes - ThorVGSharp")]
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

    [Benchmark(Description = "Multiple Shapes - SkiaSharp")]
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
}
