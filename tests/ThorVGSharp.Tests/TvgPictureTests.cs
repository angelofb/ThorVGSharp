using System.Text;

namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgPictureTests : IDisposable
{
    private static readonly byte[] SvgData = Encoding.UTF8.GetBytes("""
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16">
          <rect x="0" y="0" width="16" height="16" fill="#ff0000"/>
        </svg>
        """);

    public TvgPictureTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void Create_ReturnsPictureType()
    {
        using var picture = TvgPicture.Create();
        Assert.Equal(TvgType.Picture, picture.GetPaintType());
    }

    [Fact]
    public void LoadDataAndLoadRaw_ValidatePointerAndCopyGuards()
    {
        using var picture = TvgPicture.Create();

        Assert.Throws<ArgumentException>(() => picture.LoadData(IntPtr.Zero, 10, "image/svg+xml"));
        Assert.Throws<ArgumentException>(() => picture.LoadRaw(IntPtr.Zero, 4, 4, TvgColorSpace.Argb8888));
        Assert.Throws<ArgumentException>(() => picture.LoadData(SvgData, "image/svg+xml", copy: false));
        Assert.Throws<ArgumentException>(() => picture.LoadRaw(new uint[16], 4, 4, TvgColorSpace.Argb8888, copy: false));
    }

    [Fact]
    public void BasicPictureOperations_WorkAfterLoad()
    {
        using var picture = TvgPicture.Create();
        picture.SetAssetResolver((paint, _) =>
        {
            paint.Dispose();
            return true;
        });
        picture.LoadData(SvgData, "image/svg+xml");

        picture.SetSize(32, 48);
        var (w, h) = picture.GetSize();
        Assert.Equal(32.0f, w, 3);
        Assert.Equal(48.0f, h, 3);

        picture.SetOrigin(2, 3);
        var (x, y) = picture.GetOrigin();
        Assert.Equal(2.0f, x, 3);
        Assert.Equal(3.0f, y, 3);

        picture.SetFilter(TvgFilterMethod.Bilinear);
        picture.SetFilter(TvgFilterMethod.Nearest);

        Assert.Null(picture.GetPaint(uint.MaxValue));
    }
}
