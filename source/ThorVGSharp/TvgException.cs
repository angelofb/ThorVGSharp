using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Base class for all ThorVG exceptions.
/// </summary>
public class TvgException : Exception
{
    internal TvgException(string message) : base(message) { }
    internal TvgException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an invalid argument is passed to a ThorVG function.
/// </summary>
public sealed class TvgInvalidArgumentException : TvgException
{
    internal TvgInvalidArgumentException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when ThorVG is in an insufficient condition to perform the requested operation.
/// </summary>
public sealed class TvgInsufficientConditionException : TvgException
{
    internal TvgInsufficientConditionException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when ThorVG fails to allocate memory.
/// </summary>
public sealed class TvgFailedAllocationException : TvgException
{
    internal TvgFailedAllocationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when ThorVG detects memory corruption.
/// </summary>
public sealed class TvgMemoryCorruptionException : TvgException
{
    internal TvgMemoryCorruptionException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a requested operation is not supported.
/// </summary>
public sealed class TvgNotSupportedException : TvgException
{
    internal TvgNotSupportedException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when an unknown error occurs in ThorVG.
/// </summary>
public sealed class TvgUnknownException : TvgException
{
    internal TvgUnknownException(string message) : base(message) { }
}

/// <summary>
/// Internal helper for checking ThorVG results and throwing appropriate exceptions.
/// </summary>
internal static class TvgResultHelper
{
    /// <summary>
    /// Checks a ThorVG result and throws an exception if it's not success.
    /// </summary>
    /// <param name="result">The result to check</param>
    /// <param name="operationName">Name of the operation for error messages</param>
    internal static void CheckResult(Tvg_Result result, string operationName)
    {
        switch (result)
        {
            case Tvg_Result.TVG_RESULT_SUCCESS:
                return;
            case Tvg_Result.TVG_RESULT_INVALID_ARGUMENT:
                throw new TvgInvalidArgumentException($"Invalid argument in {operationName}");
            case Tvg_Result.TVG_RESULT_INSUFFICIENT_CONDITION:
                throw new TvgInsufficientConditionException($"Insufficient condition for {operationName}");
            case Tvg_Result.TVG_RESULT_FAILED_ALLOCATION:
                throw new TvgFailedAllocationException($"Memory allocation failed in {operationName}");
            case Tvg_Result.TVG_RESULT_MEMORY_CORRUPTION:
                throw new TvgMemoryCorruptionException($"Memory corruption detected in {operationName}");
            case Tvg_Result.TVG_RESULT_NOT_SUPPORTED:
                throw new TvgNotSupportedException($"Operation not supported: {operationName}");
            case Tvg_Result.TVG_RESULT_UNKNOWN:
            default:
                throw new TvgUnknownException($"Unknown error in {operationName}");
        }
    }
}
