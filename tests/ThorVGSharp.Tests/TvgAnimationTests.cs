namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgAnimationTests : IDisposable
{
    public TvgAnimationTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void Create_ReturnsAnimation()
    {
        using var animation = TvgAnimation.Create();
        Assert.NotNull(animation);
    }

    [Fact]
    public void PublicApi_ExposesGetPicture()
    {
        var getPicture = typeof(TvgAnimation).GetMethod(nameof(TvgAnimation.GetPicture));
        Assert.NotNull(getPicture);
    }

    [Fact]
    public void SetSegment_WithoutLoadedContent_ThrowsInsufficientCondition()
    {
        using var animation = TvgAnimation.Create();

        Assert.Throws<TvgInsufficientConditionException>(() => animation.SetSegment(1.0f, 12.0f));
    }

    [Fact]
    public void DurationAndTotalFrames_AreNonNegative()
    {
        using var animation = TvgAnimation.Create();

        Assert.True(animation.GetTotalFrames() >= 0.0f);
        Assert.True(animation.GetDuration() >= 0.0f);
    }
}
