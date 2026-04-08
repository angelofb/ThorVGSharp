namespace ThorVGSharp.Tests;

public class TvgColorStopTests
{
    [Fact]
    public void Constructor_AssignsAllFields()
    {
        var stop = new TvgColorStop(0.75f, 1, 2, 3, 4);

        Assert.Equal(0.75f, stop.Offset, 3);
        Assert.Equal((byte)1, stop.R);
        Assert.Equal((byte)2, stop.G);
        Assert.Equal((byte)3, stop.B);
        Assert.Equal((byte)4, stop.A);
    }
}
