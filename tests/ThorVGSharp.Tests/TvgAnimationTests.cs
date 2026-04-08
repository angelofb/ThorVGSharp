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

    [Fact]
    public void FrameAndSegmentApis_AreCallable()
    {
        using var animation = TvgAnimation.Create();

        TestApiAssert.AllowsTvgException(() => animation.SetFrame(2.0f));
        Assert.True(animation.GetFrame() >= 0.0f);

        var (begin, end) = animation.GetSegment();
        Assert.True(begin >= 0.0f);
        Assert.True(end >= 0.0f);
    }

    [Fact(Skip = "Known unstable with native ownership/lifetime around GetPicture(); kept for future investigation.")]
    public void FramePictureAndSegmentApis_AreCallable()
    {
        using var animation = TvgAnimation.Create();

        TestApiAssert.AllowsTvgException(() => animation.SetFrame(2.0f));
        Assert.True(animation.GetFrame() >= 0.0f);

        using var picture = animation.GetPicture();
        Assert.NotNull(picture);

        var (begin, end) = animation.GetSegment();
        Assert.True(begin >= 0.0f);
        Assert.True(end >= 0.0f);
    }

    [Fact(Skip = "[RISKY-NATIVE] Investigate GetPicture ownership across dispose ordering.")]
    public void GetPicture_DisposeOrder_AreInvestigative()
    {
        var animationA = TvgAnimation.Create();
        var pictureA = animationA.GetPicture();
        pictureA?.Dispose();
        animationA.Dispose();

        var animationB = TvgAnimation.Create();
        var pictureB = animationB.GetPicture();
        animationB.Dispose();
        pictureB?.Dispose();
    }
}
