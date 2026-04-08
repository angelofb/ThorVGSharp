using ThorVGSharp.Interop;

namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgSaverTests : IDisposable
{
    public TvgSaverTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void Create_ReturnsSaver()
    {
        using var saver = TvgSaver.Create();
        Assert.NotNull(saver);
    }

    [Fact]
    public void Save_ThrowsOnNullArguments()
    {
        using var saver = TvgSaver.Create();

        Assert.Throws<ArgumentNullException>(() => saver.Save((TvgPaint)null!, "test.tvg"));
        Assert.Throws<ArgumentNullException>(() => saver.Save((TvgAnimation)null!, "test.gif"));
    }

    [Fact]
    public unsafe void SaveAndSync_WithNullNativeHandle_ReturnTvgException()
    {
        using var saver = new TvgSaver((_Tvg_Saver*)0);
        using var shape = TvgShape.Create();
        using var animation = TvgAnimation.Create();

        shape.AppendRect(0, 0, 16, 16);
        shape.SetFillColor(255, 0, 0, 255);

        TestApiAssert.AllowsTvgException(() => saver.Save(shape, "missing-output.svg"));
        TestApiAssert.AllowsTvgException(() => saver.Save(animation, "missing-output.gif", quality: 90, fps: 24));
        TestApiAssert.AllowsTvgException(() => saver.Sync());
    }
}
