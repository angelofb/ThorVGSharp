# ThorVGSharp Benchmarks

Performance benchmarks comparing ThorVGSharp with SkiaSharp for vector graphics rendering.

## Test Conditions

These benchmarks are designed to provide a fair comparison between ThorVGSharp and SkiaSharp under similar conditions.

### Environment
- **Framework**: .NET 10.0
- **Architecture**: x64
- **Benchmark Tool**: BenchmarkDotNet 0.14.0
- **Benchmarked Libraries**:
  - ThorVGSharp (using native ThorVG)
  - SkiaSharp 3.116.1

### Test Scenarios

#### 1. Shape Rendering (`ShapeRenderingBenchmark`)
Tests basic shape rendering performance with simple primitives:
- **Simple Rectangle**: Single filled rectangle (800x600 canvas)
- **Circle**: Single filled circle (800x600 canvas)
- **Complex Path**: Path with multiple curves and lines (800x600 canvas)

#### 2. Complex Scene Rendering (`ComplexSceneBenchmark`)
Tests performance with multiple objects and effects:
- **100 Shapes**: Rendering 100 rectangles with rounded corners (1920x1080 canvas)
- **Gradients**: 20 shapes with linear gradients (1920x1080 canvas)

### Rendering Configuration

**ThorVGSharp:**
- Software rasterizer (SwCanvas)
- ARGB8888 color space
- Direct buffer rendering

**SkiaSharp:**
- Software rasterizer (SkSurface.Create)
- RGBA8888 color space
- Anti-aliasing enabled

### Hardware Specifications

Run benchmarks on your machine to get results specific to your hardware. Typical test environment:
- **CPU**: Modern x64 processor (4+ cores recommended)
- **RAM**: 8GB+ recommended
- **OS**: Windows 10/11, Linux, or macOS

## Running Benchmarks

### Prerequisites
```bash
dotnet restore
```

### Run All Benchmarks
```bash
cd benchs/ThorVGSharp.Benchmarks
dotnet run -c Release
```

### Run Specific Benchmark
```bash
dotnet run -c Release --filter "*ShapeRendering*"
```

### Run with Custom BenchmarkDotNet Arguments
```bash
dotnet run -c Release -- --filter "*Circle*" --memory
```

## Benchmark Output

BenchmarkDotNet generates detailed reports including:
- **Mean execution time** per operation
- **Standard deviation** and confidence intervals
- **Memory allocation** statistics
- **Rank** comparison between methods
- **Detailed statistical analysis**

Results are saved in `BenchmarkDotNet.Artifacts/results/` directory.

## Understanding Results

### Metrics
- **Mean**: Average execution time
- **Error**: Half of 99.9% confidence interval
- **StdDev**: Standard deviation of all measurements
- **Median**: 50th percentile
- **Allocated**: Total memory allocated

### Memory Diagnostics
Memory diagnostics are enabled by default (`[MemoryDiagnoser]`), showing:
- Gen0/Gen1/Gen2 collections
- Total bytes allocated
- Memory pressure

## Comparison Notes

### ThorVG Strengths
- Optimized for vector graphics rendering
- Efficient memory usage for complex scenes
- Native performance with C# wrapper

### SkiaSharp Strengths
- Mature ecosystem with extensive features
- Hardware acceleration support (GPU)
- Rich API with many rendering options

## Methodology

These benchmarks follow best practices:
1. **Warmup**: Multiple iterations to warm up JIT and caches
2. **Measurements**: Multiple measurements for statistical significance
3. **Isolation**: Each benchmark runs in isolation
4. **Fair Comparison**: Similar rendering quality and features
5. **Reproducibility**: Fixed random seeds and deterministic operations

## Contributing

To add new benchmarks:
1. Create a new class inheriting benchmark attributes
2. Add `[Benchmark]` methods for each test case
3. Include both ThorVG and SkiaSharp implementations
4. Update this README with test descriptions

## License

These benchmarks are part of the ThorVGSharp project and follow the same MIT license.

## References

- [ThorVG Performance](https://www.thorvg.org/about)
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [SkiaSharp Documentation](https://learn.microsoft.com/en-us/dotnet/api/skiasharp)
