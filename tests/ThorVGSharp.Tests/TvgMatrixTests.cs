namespace ThorVGSharp.Tests;

public class TvgMatrixTests
{
    [Fact]
    public void Constructor_AssignsAllElements()
    {
        var matrix = new TvgMatrix(1, 2, 3, 4, 5, 6, 7, 8, 9);

        Assert.Equal(1.0f, matrix.E11, 3);
        Assert.Equal(2.0f, matrix.E12, 3);
        Assert.Equal(3.0f, matrix.E13, 3);
        Assert.Equal(4.0f, matrix.E21, 3);
        Assert.Equal(5.0f, matrix.E22, 3);
        Assert.Equal(6.0f, matrix.E23, 3);
        Assert.Equal(7.0f, matrix.E31, 3);
        Assert.Equal(8.0f, matrix.E32, 3);
        Assert.Equal(9.0f, matrix.E33, 3);
    }
}
