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

    [Fact(Skip = "Environment-dependent (WebGPU backend/device); retained for manual investigation.")]
    public void Create_IsCallable()
    {
        var ex = Record.Exception(() =>
        {
            using var canvas = TvgCanvasWebGPU.Create();
            Assert.NotNull(canvas);
        });

        if (ex is not null)
            Assert.IsAssignableFrom<TvgException>(ex);
    }

    [Fact(Skip = "[RISKY-ENV] Requires valid WebGPU device/instance/target to validate SetTarget.")]
    public void SetTarget_WithRealDevice_AreInvestigative()
    {
        using var canvas = TvgCanvasWebGPU.Create();
        canvas.SetTarget(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 640, 480, TvgColorSpace.Argb8888, type: 0);
    }
}
