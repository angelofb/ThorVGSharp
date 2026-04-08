namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgLottieAnimationTests : IDisposable
{
    public TvgLottieAnimationTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void Create_ReturnsAnimationWithPicture()
    {
        using var animation = TvgLottieAnimation.Create();
        Assert.NotNull(animation);

        using var picture = animation.GetPicture();
        Assert.NotNull(picture);
        Assert.Equal(TvgType.Picture, picture!.GetPaintType());
    }

    [Fact]
    public void MarkerQueries_OnEmptyAnimation_ReturnDefaults()
    {
        using var animation = TvgLottieAnimation.Create();

        Assert.Equal(0u, animation.GetMarkersCount());
        Assert.Null(animation.GetMarker(0));
        Assert.Null(animation.GetMarkerInfo(0));
    }

    [Fact]
    public void SetSegment_WithoutLoadedContent_ThrowsInsufficientCondition()
    {
        using var animation = TvgLottieAnimation.Create();

        Assert.Throws<TvgInsufficientConditionException>(() => animation.SetSegment(2.0f, 8.0f));
    }
}
