namespace ThorVGSharp.Tests;

public class TvgMarkerInfoTests
{
    [Fact]
    public void Constructor_AssignsFields()
    {
        var info = new TvgMarkerInfo("intro", 0.0f, 24.0f);

        Assert.Equal("intro", info.Name);
        Assert.Equal(0.0f, info.Begin, 3);
        Assert.Equal(24.0f, info.End, 3);
    }

    [Fact]
    public void Constructor_ThrowsOnNullName()
    {
        Assert.Throws<ArgumentNullException>(() => new TvgMarkerInfo(null!, 0.0f, 1.0f));
    }
}
