namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgFontManagerTests : IDisposable
{
    public TvgFontManagerTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void LoadData_ManagedOverload_ValidatesInputs()
    {
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("", new byte[] { 1, 2, 3 }));
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("MyFont", ReadOnlySpan<byte>.Empty));
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("MyFont", new byte[] { 1, 2, 3 }, copy: false));
    }

    [Fact]
    public void LoadData_PointerOverload_ValidatesInputs()
    {
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("", IntPtr.Zero, 0));
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("MyFont", IntPtr.Zero, 12));
    }
}
