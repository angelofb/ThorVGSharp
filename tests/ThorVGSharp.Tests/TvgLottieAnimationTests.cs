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

    [Fact]
    public void SlotMarkerAndPlaybackApis_AreCallable()
    {
        using var animation = TvgLottieAnimation.Create();

        uint slotId = TestApiAssert.AllowsTvgException(() => animation.GenerateSlot("slot"));
        _ = TestApiAssert.AllowsTvgException(() => animation.GenerateSlot(new string('x', 600)));

        TestApiAssert.AllowsTvgException(() => animation.ApplySlot(slotId));
        TestApiAssert.AllowsTvgException(() => animation.DeleteSlot(slotId));
        TestApiAssert.AllowsTvgException(() => animation.SetMarker("intro"));
        TestApiAssert.AllowsTvgException(() => animation.SetMarker(new string('m', 600)));
        TestApiAssert.AllowsTvgException(() => animation.Tween(0.0f, 10.0f, 0.5f));

        TestApiAssert.AllowsTvgException(() => animation.Assign("layer", 0, "opacity", 0.75f));
        TestApiAssert.AllowsTvgException(() => animation.Assign(new string('l', 600), 1, new string('v', 600), 0.3f));

        TestApiAssert.AllowsTvgException(() => animation.SetQuality(80));
        TestApiAssert.AllowsTvgException(() => animation.SetFrame(3.0f));
        _ = TestApiAssert.AllowsTvgException(() => animation.GetFrame());
        _ = TestApiAssert.AllowsTvgException(() => animation.GetTotalFrames());
        _ = TestApiAssert.AllowsTvgException(() => animation.GetDuration());
        _ = TestApiAssert.AllowsTvgException(() => animation.GetSegment());
    }
}
