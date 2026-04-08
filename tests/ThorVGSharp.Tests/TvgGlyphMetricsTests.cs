namespace ThorVGSharp.Tests;

public class TvgGlyphMetricsTests
{
    [Fact]
    public void Constructor_AssignsAllFields()
    {
        var min = new TvgPoint(1.0f, 2.0f);
        var max = new TvgPoint(3.0f, 4.0f);
        var metrics = new TvgGlyphMetrics(5.0f, 6.0f, min, max);

        Assert.Equal(5.0f, metrics.Advance, 3);
        Assert.Equal(6.0f, metrics.Bearing, 3);
        Assert.Equal(1.0f, metrics.Min.X, 3);
        Assert.Equal(2.0f, metrics.Min.Y, 3);
        Assert.Equal(3.0f, metrics.Max.X, 3);
        Assert.Equal(4.0f, metrics.Max.Y, 3);
    }
}
