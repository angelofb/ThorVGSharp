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
}
