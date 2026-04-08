namespace ThorVGSharp.Tests;

public class TvgPointTests
{
    [Fact]
    public void Constructor_AssignsCoordinates()
    {
        var point = new TvgPoint(12.5f, -3.25f);

        Assert.Equal(12.5f, point.X, 3);
        Assert.Equal(-3.25f, point.Y, 3);
    }
}
