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

    [Fact]
    public void TextMetricsApis_AreCallable()
    {
        using var text = TvgText.Create();
        text.SetText("A");

        Assert.True(text.GetLineCount() >= 0);

        _ = TestApiAssert.AllowsTvgException(() => text.GetTextMetrics());
        _ = TestApiAssert.AllowsTvgException(() => text.GetGlyphMetrics("A"));
    }

    [Fact]
    public void SetSize_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.SetSize(18.0f));
    }

    [Fact]
    public void Align_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.Align(10.0f, 20.0f));
    }

    [Fact]
    public void Layout_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.Layout(320.0f, 120.0f));
    }

    [Fact]
    public void WrapMode_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.SetWrapMode(TvgTextWrap.Word));
    }

    [Fact]
    public void Spacing_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.SetSpacing(1.2f, 1.1f));
    }

    [Fact]
    public void Italic_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.SetItalic(8.0f));
    }

    [Fact]
    public void Outline_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.SetOutline(1.0f, 20, 30, 40));
    }

    [Fact]
    public void Color_ApiIsCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");
        TestApiAssert.AllowsTvgException(() => text.SetColor(100, 120, 140));
    }

    [Fact(Skip = "Known hang/flaky on native runtime: investigate text style/layout chain (see blame-hang history).")]
    public void StylingAndLayoutApis_AreCallable()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");

        TestApiAssert.AllowsTvgException(() => text.SetFont("Sans"));
        TestApiAssert.AllowsTvgException(() => text.SetSize(18.0f));
        TestApiAssert.AllowsTvgException(() => text.Align(10.0f, 20.0f));
        TestApiAssert.AllowsTvgException(() => text.Layout(320.0f, 120.0f));
        TestApiAssert.AllowsTvgException(() => text.SetWrapMode(TvgTextWrap.Word));
        TestApiAssert.AllowsTvgException(() => text.SetSpacing(1.2f, 1.1f));
        TestApiAssert.AllowsTvgException(() => text.SetItalic(8.0f));
        TestApiAssert.AllowsTvgException(() => text.SetOutline(1.0f, 20, 30, 40));
        TestApiAssert.AllowsTvgException(() => text.SetColor(100, 120, 140));

        using var gradient = TvgLinearGradient.Create();
        gradient.SetLinear(0, 0, 100, 0);
        TestApiAssert.AllowsTvgException(() => text.SetGradient(gradient));
        TestApiAssert.AllowsTvgException(() => text.SetGradient(null));
    }

    [Fact(Skip = "[RISKY-NATIVE] Investigate SetFont behavior with unloaded/UTF-8/long font names.")]
    public void SetFont_UnloadedAndLongUtf8Names_AreInvestigative()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");

        TestApiAssert.AllowsTvgException(() => text.SetFont("Sans"));
        TestApiAssert.AllowsTvgException(() => text.SetFont("Noto Sans CJK JP"));
        TestApiAssert.AllowsTvgException(() => text.SetFont("テストフォント"));
        TestApiAssert.AllowsTvgException(() => text.SetFont(new string('F', 1024)));
    }

    [Fact(Skip = "[RISKY-NATIVE] Investigate text gradient attach/detach with dispose ordering.")]
    public void SetGradient_AttachDetach_DisposeOrder_AreInvestigative()
    {
        using var text = TvgText.Create();
        text.SetText("ThorVG");

        var gradA = TvgLinearGradient.Create();
        gradA.SetLinear(0, 0, 120, 0);
        TestApiAssert.AllowsTvgException(() => text.SetGradient(gradA));
        gradA.Dispose();

        using var gradB = TvgLinearGradient.Create();
        gradB.SetLinear(0, 0, 0, 120);
        TestApiAssert.AllowsTvgException(() => text.SetGradient(gradB));
        TestApiAssert.AllowsTvgException(() => text.SetGradient(null));
    }

}
