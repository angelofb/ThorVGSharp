using ThorVGSharp;

namespace ThorVGSharp.Sample.Showcase.Examples;

/// <summary>
/// Animation Example - Demonstrates ThorVG Lottie animation playback
/// Ported from: thorvg.example/src/Animation.cpp
/// </summary>
internal class AnimationExample : Example
{
    private TvgAnimation? _animation;
    private TvgShape? _background;

    public override bool Content(TvgCanvas canvas, uint width, uint height)
    {
        // Load fallback font
        var fontData = LoadResource("font/PublicSans-Regular.ttf");
        TvgFontManager.LoadData("PublicSans-Regular", fontData, "font/ttf");

        // Background
        _background = TvgShape.Create();
        _background.AppendRect(0, 0, width, height);
        _background.SetFillColor(50, 50, 50);
        canvas.Add(_background);

        // Load Lottie animation
        _animation = TvgAnimation.Create();
        var picture = _animation.GetPicture();
        if (picture == null)
        {
            Console.WriteLine("Failed to get picture from animation");
            return false;
        }

        picture.SetOrigin(0.5f, 0.5f); // Center origin

        var lottieData = LoadResource("lottie/sample.json");
        try
        {
            picture.LoadData(lottieData, "lottie");
        }
        catch
        {
            Console.WriteLine("Failed to load Lottie animation");
            return false;
        }

        // Scale to fit window while preserving aspect ratio
        var (w, h) = picture.GetSize();
        var scale = (w > h) ? width / w : height / h;
        picture.Scale(scale);
        picture.Translate(width * 0.5f, height * 0.5f);

        canvas.Add(picture);
        return true;
    }

    public override bool Update(TvgCanvas canvas, uint elapsed)
    {
        if (_animation == null) return false;

        var progress = AnimationHelper.Progress(elapsed, _animation.GetDuration());

        // Update animation frame only when it's changed
        var frame = _animation.GetTotalFrames() * progress;
        try
        {
            _animation.SetFrame(frame);
            canvas.Update();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override void Dispose()
    {
        _background?.Dispose();
        _animation?.Dispose();
        base.Dispose();
    }
}
