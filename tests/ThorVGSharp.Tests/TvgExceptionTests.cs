using ThorVGSharp.Interop;

namespace ThorVGSharp.Tests;

public class TvgExceptionTests
{
    public static IEnumerable<object[]> ErrorMappings =>
    [
        [(uint)Tvg_Result.TVG_RESULT_INVALID_ARGUMENT, typeof(TvgInvalidArgumentException)],
        [(uint)Tvg_Result.TVG_RESULT_INSUFFICIENT_CONDITION, typeof(TvgInsufficientConditionException)],
        [(uint)Tvg_Result.TVG_RESULT_FAILED_ALLOCATION, typeof(TvgFailedAllocationException)],
        [(uint)Tvg_Result.TVG_RESULT_MEMORY_CORRUPTION, typeof(TvgMemoryCorruptionException)],
        [(uint)Tvg_Result.TVG_RESULT_NOT_SUPPORTED, typeof(TvgNotSupportedException)],
        [(uint)Tvg_Result.TVG_RESULT_UNKNOWN, typeof(TvgUnknownException)],
    ];

    [Fact]
    public void CheckResult_Success_DoesNotThrow()
    {
        TvgResultHelper.CheckResult(Tvg_Result.TVG_RESULT_SUCCESS, "test op");
    }

    [Theory]
    [MemberData(nameof(ErrorMappings))]
    public void CheckResult_Error_ThrowsExpectedException(uint resultValue, Type expectedExceptionType)
    {
        var result = (Tvg_Result)resultValue;
        var ex = Assert.Throws(expectedExceptionType, () => TvgResultHelper.CheckResult(result, "test op"));
        Assert.Contains("test op", ex.Message);
    }
}
