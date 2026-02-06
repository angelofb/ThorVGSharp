using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using SkiaSharp;
using ThorVGSharp;

namespace ThorVGSharp.Benchmarks;

/// <summary>
/// Benchmark comparing simple shape rendering between ThorVGSharp and SkiaSharp.
/// Based on ThorVG performance tests from https://www.thorvg.org/about
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ShapeRenderingBenchmark
{
    private uint[]? _thorvgBuffer;
    private SKBitmap? _skiaBitmap;
    private const int Width = 800;
    private const int Height = 600;

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

    [Benchmark(Description = "ThorVG - Simple Rectangle")]
    public void ThorVG_SimpleRectangle()
    {
        using var canvas = TvgCanvasSoftware.Create();
        canvas!.SetTarget(_thorvgBuffer!, (uint)Width, (uint)Width, (uint)Height, TvgColorSpace.Argb8888);

        using var shape = TvgShape.Create();
        shape!.AppendRect(100, 100, 600, 400, 0, 0);
        shape.SetFillColor(255, 100, 100, 255);

        canvas.Add(shape);
        canvas.Draw();
        canvas.Sync();
    }

    [Benchmark(Description = "SkiaSharp - Simple Rectangle")]
    public void SkiaSharp_SimpleRectangle()
    {
        using var surface = SKSurface.Create(_skiaBitmap!.Info, _skiaBitmap.GetPixels(), _skiaBitmap.RowBytes);
        using var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint
        {
            Color = new SKColor(255, 100, 100, 255),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        canvas.DrawRect(100, 100, 600, 400, paint);
        surface.Flush();
    }

    [Benchmark(Description = "ThorVG - Circle")]
    public void ThorVG_Circle()
    {
        using var canvas = TvgCanvasSoftware.Create();
        canvas!.SetTarget(_thorvgBuffer!, (uint)Width, (uint)Width, (uint)Height, TvgColorSpace.Argb8888);

        using var shape = TvgShape.Create();
        shape!.AppendCircle(400, 300, 200, 200);
        shape.SetFillColor(100, 200, 255, 255);

        canvas.Add(shape);
        canvas.Draw();
        canvas.Sync();
    }

    [Benchmark(Description = "SkiaSharp - Circle")]
    public void SkiaSharp_Circle()
    {
        using var surface = SKSurface.Create(_skiaBitmap!.Info, _skiaBitmap.GetPixels(), _skiaBitmap.RowBytes);
        using var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint
        {
            Color = new SKColor(100, 200, 255, 255),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        canvas.DrawCircle(400, 300, 200, paint);
        surface.Flush();
    }

    [Benchmark(Description = "ThorVG - Complex Path")]
    public void ThorVG_ComplexPath()
    {
        using var canvas = TvgCanvasSoftware.Create();
        canvas!.SetTarget(_thorvgBuffer!, (uint)Width, (uint)Width, (uint)Height, TvgColorSpace.Argb8888);

        using var shape = TvgShape.Create();
        shape!.MoveTo(100, 100);
        shape.LineTo(700, 100);
        shape.CubicTo(700, 200, 600, 300, 500, 400);
        shape.LineTo(300, 500);
        shape.CubicTo(200, 400, 100, 300, 100, 200);
        shape.Close();
        shape.SetFillColor(200, 150, 100, 255);

        canvas.Add(shape);
        canvas.Draw();
        canvas.Sync();
    }

    [Benchmark(Description = "SkiaSharp - Complex Path")]
    public void SkiaSharp_ComplexPath()
    {
        using var surface = SKSurface.Create(_skiaBitmap!.Info, _skiaBitmap.GetPixels(), _skiaBitmap.RowBytes);
        using var canvas = surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        using var path = new SKPath();
        path.MoveTo(100, 100);
        path.LineTo(700, 100);
        path.CubicTo(700, 200, 600, 300, 500, 400);
        path.LineTo(300, 500);
        path.CubicTo(200, 400, 100, 300, 100, 200);
        path.Close();

        using var paint = new SKPaint
        {
            Color = new SKColor(200, 150, 100, 255),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        canvas.DrawPath(path, paint);
        surface.Flush();
    }
}
