namespace ThorVGSharp.Tests;

public class TvgCanvasOpenGLTests
{
    [Fact]
    public void Type_InheritsFromCanvas()
    {
        Assert.True(typeof(TvgCanvas).IsAssignableFrom(typeof(TvgCanvasOpenGL)));
    }

    [Fact]
    public void PublicApi_ExposesCreateAndSetTarget()
    {
        var create = typeof(TvgCanvasOpenGL).GetMethod(nameof(TvgCanvasOpenGL.Create));
        var setTarget = typeof(TvgCanvasOpenGL).GetMethod(nameof(TvgCanvasOpenGL.SetTarget));

        Assert.NotNull(create);
        Assert.True(create!.IsStatic);
        Assert.NotNull(setTarget);
    }
}
