using BenchmarkDotNet.Running;

using ThorVGSharp.Benchmarks;

// Run all benchmarks or specific ones based on command line arguments
if (args.Length > 0 && args[0] == "--filter")
{
    var filter = args.Length > 1 ? args[1] : "*";
    BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
else
{
    // Run all benchmarks by default
    BenchmarkRunner.Run<ShapeRenderingBenchmark>();
    BenchmarkRunner.Run<ComplexSceneBenchmark>();
    BenchmarkRunner.Run<GradientsBenchmark>();
}
