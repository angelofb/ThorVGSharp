// Tests adapted from external/thorvg/test/testSwCanvas.cpp
namespace ThorVGSharp.Tests;

public class TvgCanvasSoftwareTests : IDisposable
{
    public TvgCanvasSoftwareTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void BasicCreation()
    {
        using var canvas = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas);

        using var canvas2 = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas2);

        using var canvas3 = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas3);
    }

    [Fact]
    public void TargetBuffer()
    {
        using var canvas = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas);

        var buffer = new uint[100 * 100];
        canvas.SetTarget(buffer, 100, 100, 100, TvgColorSpace.Argb8888);
        canvas.SetTarget(buffer, 100, 100, 100, TvgColorSpace.Argb8888); // Should not throw

        // Invalid arguments should throw
        Assert.Throws<TvgInvalidArgumentException>(() =>
            canvas.SetTarget(IntPtr.Zero, 100, 100, 100, TvgColorSpace.Argb8888));
        Assert.Throws<TvgInvalidArgumentException>(() =>
            canvas.SetTarget(buffer, 0, 100, 100, TvgColorSpace.Argb8888));
        Assert.Throws<TvgInvalidArgumentException>(() =>
            canvas.SetTarget(buffer, 100, 0, 100, TvgColorSpace.Argb8888));
        Assert.Throws<TvgInvalidArgumentException>(() =>
            canvas.SetTarget(buffer, 100, 200, 100, TvgColorSpace.Argb8888)); // stride mismatch
        Assert.Throws<TvgInvalidArgumentException>(() =>
            canvas.SetTarget(buffer, 100, 100, 0, TvgColorSpace.Argb8888));
    }

    [Fact]
    public void PushingPaints()
    {
        using var canvas = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas);

        var buffer = new uint[100 * 100];
        canvas.SetTarget(buffer, 100, 100, 100, TvgColorSpace.Argb8888);

        // Try all types of paints
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);
        canvas.Add(shape);

        using var picture = TvgPicture.Create();
        Assert.NotNull(picture);
        canvas.Add(picture);

        using var scene = TvgScene.Create();
        Assert.NotNull(scene);
        canvas.Add(scene);

        // Update and add more
        canvas.Update();

        using var shape2 = TvgShape.Create();
        Assert.NotNull(shape2);
        canvas.Add(shape2);

        using var shape3 = TvgShape.Create();
        Assert.NotNull(shape3);
        canvas.Add(shape3);

        // Null argument should throw
        Assert.Throws<ArgumentNullException>(() => canvas.Add(null!));
    }

    [Fact]
    public void DrawAndUpdate()
    {
        using var canvas = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas);

        var buffer = new uint[100 * 100];
        canvas.SetTarget(buffer, 100, 100, 100, TvgColorSpace.Argb8888);

        using var shape = TvgShape.Create();
        Assert.NotNull(shape);
        shape.AppendRect(10, 10, 80, 80, 0, 0);
        shape.SetFillColor(255, 0, 0, 255);

        canvas.Add(shape);
        canvas.Update();
        canvas.Draw();
        canvas.Sync();
    }

    [Fact]
    public void InsertRemoveAndViewportApis_AreCallable()
    {
        using var canvas = TvgCanvasSoftware.Create();
        var buffer = new uint[64 * 64];
        canvas.SetTarget(buffer, 64, 64, 64, TvgColorSpace.Argb8888);

        using var first = TvgShape.Create();
        using var second = TvgShape.Create();

        first.AppendRect(2, 2, 20, 20);
        second.AppendCircle(16, 16, 8);

        canvas.Add(first);
        TestApiAssert.AllowsTvgException(() => canvas.Insert(second, first));
        TestApiAssert.AllowsTvgException(() => canvas.Insert(second));
        TestApiAssert.AllowsTvgException(() => canvas.Remove(second));
        TestApiAssert.AllowsTvgException(() => canvas.SetViewport(0, 0, 48, 48));

        Assert.Throws<ArgumentNullException>(() => canvas.Remove(null!));
        Assert.Throws<ArgumentNullException>(() => canvas.Insert(null!, first));
    }
}
