namespace ThorVGSharp.Tests;

public class TvgCanvasWebGPUTests
{
    [Fact]
    public void Type_InheritsFromCanvas()
    {
        Assert.True(typeof(TvgCanvas).IsAssignableFrom(typeof(TvgCanvasWebGPU)));
    }

    [Fact]
    public void PublicApi_ExposesCreateAndSetTarget()
    {
        var create = typeof(TvgCanvasWebGPU).GetMethod(nameof(TvgCanvasWebGPU.Create));
        var setTarget = typeof(TvgCanvasWebGPU).GetMethod(nameof(TvgCanvasWebGPU.SetTarget));

        Assert.NotNull(create);
        Assert.True(create!.IsStatic);
        Assert.NotNull(setTarget);
    }
}
