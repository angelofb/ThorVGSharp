namespace ThorVGSharp.Tests;

/// <summary>
/// Collection definition for tests that need serial execution due to shared TvgEngine state.
/// </summary>
[CollectionDefinition("TvgEngine", DisableParallelization = true)]
public class TvgEngineCollection
{
}
