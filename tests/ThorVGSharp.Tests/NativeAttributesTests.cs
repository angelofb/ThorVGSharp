using ThorVGSharp.Internal.Attributes;

namespace ThorVGSharp.Tests;

public class NativeAttributesTests
{
    [Fact]
    public void NativeAnnotationAttribute_StoresAnnotation()
    {
        var attr = new NativeAnnotationAttribute("nonnull");
        Assert.Equal("nonnull", attr.Annotation);
    }

    [Fact]
    public void NativeBitfieldAttribute_StoresValues()
    {
        var attr = new NativeBitfieldAttribute("flags", 3, 5);
        Assert.Equal("flags", attr.Name);
        Assert.Equal(3, attr.Offset);
        Assert.Equal(5, attr.Length);
    }

    [Fact]
    public void NativeTypeNameAttribute_StoresName()
    {
        var attr = new NativeTypeNameAttribute("uint32_t");
        Assert.Equal("uint32_t", attr.Name);
    }
}
