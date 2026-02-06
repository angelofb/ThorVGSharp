# ThorVGSharp Tests

Unit tests for ThorVGSharp, adapted from the original ThorVG C++ test suite.

## Test Organization

Tests are organized by component:

- **TvgEngineTests.cs** - Engine initialization and versioning (from `testInitializer.cpp`)
- **TvgShapeTests.cs** - Shape creation and manipulation (from `testShape.cpp`)
- **TvgCanvasSoftwareTests.cs** - Software canvas rendering (from `testSwCanvas.cpp`)
- **TvgSceneTests.cs** - Scene management and hierarchy (from `testScene.cpp`)

## Running Tests

### Basic test run
```bash
dotnet test
```

### Run with code coverage
```bash
dotnet test /p:CollectCoverage=true
```

### Generate coverage report (multiple formats)
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura,opencover,json
```

### View coverage summary
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

Coverage reports are generated in `tests/ThorVGSharp.Tests/coverage/` directory.

## Coverage Reports

The test project is configured to generate coverage in multiple formats:
- **Cobertura** - XML format compatible with most CI/CD systems
- **OpenCover** - XML format for detailed analysis
- **JSON** - Machine-readable format for custom tools

## Test Framework

- **xUnit** v3.2.2 - Test framework
- **Coverlet** v6.0.4 - Code coverage tool

## Original Test Suite

These tests are adapted from the ThorVG C++ test suite located in `external/thorvg/test/`.
The tests have been translated to C# and adapted to use xUnit and ThorVGSharp's managed API.
