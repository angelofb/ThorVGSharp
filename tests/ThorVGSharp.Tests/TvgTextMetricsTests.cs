namespace ThorVGSharp.Tests;

public class TvgTextMetricsTests
{
    [Fact]
    public void Constructor_AssignsAllFields()
    {
        var metrics = new TvgTextMetrics(1.0f, 2.0f, 3.0f, 4.0f);

        Assert.Equal(1.0f, metrics.Ascent, 3);
        Assert.Equal(2.0f, metrics.Descent, 3);
        Assert.Equal(3.0f, metrics.LineGap, 3);
        Assert.Equal(4.0f, metrics.Advance, 3);
    }
}
