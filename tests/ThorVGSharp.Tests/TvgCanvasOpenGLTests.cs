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

    [Fact(Skip = "Environment-dependent (OpenGL context/driver); retained for manual investigation.")]
    public void Create_IsCallable()
    {
        var ex = Record.Exception(() =>
        {
            using var canvas = TvgCanvasOpenGL.Create();
            Assert.NotNull(canvas);
        });

        if (ex is not null)
            Assert.IsAssignableFrom<TvgException>(ex);
    }

    [Fact(Skip = "[RISKY-ENV] Requires valid OpenGL display/surface/context to validate SetTarget.")]
    public void SetTarget_WithRealContext_AreInvestigative()
    {
        using var canvas = TvgCanvasOpenGL.Create();
        canvas.SetTarget(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, 640, 480, TvgColorSpace.Argb8888);
    }
}
