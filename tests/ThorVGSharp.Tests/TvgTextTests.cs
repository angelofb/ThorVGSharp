namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgTextTests : IDisposable
{
    public TvgTextTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void Create_ReturnsTextType()
    {
        using var text = TvgText.Create();
        Assert.Equal(TvgType.Text, text.GetPaintType());
    }

    [Fact]
    public void SetText_StoresUtf8Content()
    {
        using var text = TvgText.Create();
        text.SetText("Ciao ThorVG");

        var value = text.GetText();
        Assert.NotNull(value);
        Assert.Contains("ThorVG", value!);
    }

    [Fact]
    public void GetGlyphMetrics_ThrowsOnEmptyGlyph()
    {
        using var text = TvgText.Create();
        Assert.Throws<ArgumentException>(() => text.GetGlyphMetrics(string.Empty));
    }

}
