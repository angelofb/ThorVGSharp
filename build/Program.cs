using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context)
        : base(context)
    {
        RootDirectory = context.Environment.WorkingDirectory.GetParent();
        SourceDirectory = RootDirectory.Combine("source");
        ExternalDirectory = RootDirectory.Combine("external");
    }

    public DirectoryPath RootDirectory { get; }
    public DirectoryPath SourceDirectory { get; }
    public DirectoryPath ExternalDirectory { get; }
    public DirectoryPath ThorVGDirectory => ExternalDirectory.Combine("thorvg");
    public FilePath BindingsDirectory => SourceDirectory
                    .Combine("ThorVGSharp")
                    .Combine("Internal")
                    .CombineWithFilePath("NativeBindings.g.cs");
}

[TaskName("bindings")]
[TaskDescription("Generates P/Invoke bindings from ThorVG C API using ClangSharp")]
public sealed class GenerateBindingsTask : FrostingTask<BuildContext>
{
    private const string ClangSharpToolId = "clangsharppinvokegenerator";

    public override void Run(BuildContext context)
    {
        context.Information("Generating P/Invoke bindings from thorvg_capi.h...");

        var headerPath = context.ThorVGDirectory.CombineWithFilePath("src/bindings/capi/thorvg_capi.h");
        var outputPath = context.BindingsDirectory.FullPath;

        if (!context.FileExists(headerPath))
        {
            context.Error($"ThorVG header not found at: {headerPath}");
            throw new FileNotFoundException("thorvg_capi.h not found", headerPath.FullPath);
        }

        // Build the ClangSharp command line arguments
        List<string> clangSharpArgs =
        [
            "--file", headerPath.FullPath,
            "--output", outputPath,
            "--namespace", "ThorVGSharp.Interop",
            "--libraryPath", "thorvg",
            "--methodClassName", "NativeMethods",
            "--with-access-specifier", "*=Internal",
            "--with-using", "*=ThorVGSharp.Internal.Attributes",
            "--config", "latest-codegen",
            "--config", "single-file",
            "--config", "generate-native-bitfield-attribute",
            "--config", "generate-macro-bindings",
            "--config", "generate-aggressive-inlining",
            "--config", "generate-unmanaged-constants",
            "--exclude", "TVG_API",
            "--exclude", "TVG_DEPRECATED",
            "--remap", "Tvg_Point=TvgPoint",
            "--remap", "Tvg_Matrix=TvgMatrix",
            "--remap", "Tvg_Color_Stop=TvgColorStop",
            "--exclude", "Tvg_Point",
            "--exclude", "Tvg_Matrix",
            "--exclude", "Tvg_Color_Stop",
        ];

        AddMacOSSdkArguments(context, clangSharpArgs);

        context.Information("Running ClangSharpPInvokeGenerator...");

        // Ensure dotnet tools are restored
        context.StartProcess("dotnet", new ProcessSettings
        {
            Arguments = "tool restore",
            WorkingDirectory = context.RootDirectory
        });

        // Build dotnet tool run command
        var arguments = new ProcessArgumentBuilder()
            .Append("tool")
            .Append("run")
            .Append("ClangSharpPInvokeGenerator")
            .Append("--");

        foreach (var arg in clangSharpArgs)
        {
            arguments.Append(arg);
        }

        var processSettings = new ProcessSettings
        {
            Arguments = arguments,
            WorkingDirectory = context.RootDirectory
        };

        ConfigureMacOSLibClangPath(context, processSettings);

        var exitCode = context.StartProcess("dotnet", processSettings);

        if (exitCode != 0)
        {
            throw new Exception($"ClangSharpPInvokeGenerator failed with exit code {exitCode}");
        }

        context.Information($"Bindings generated successfully in: {outputPath}");
    }

    private static void ConfigureMacOSLibClangPath(BuildContext context, ProcessSettings processSettings)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        string toolVersion = TryReadToolVersion(context.RootDirectory.FullPath);
        if (string.IsNullOrWhiteSpace(toolVersion))
        {
            context.Warning("Unable to read ClangSharp tool version from dotnet-tools.json; skipping macOS libclang setup.");
            return;
        }

        string rid = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.Arm64 => "osx-arm64",
            Architecture.X64 => "osx-x64",
            _ => null
        };

        if (rid is null)
        {
            context.Warning($"Unsupported macOS architecture '{RuntimeInformation.ProcessArchitecture}' for ClangSharp libclang setup.");
            return;
        }

        var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var toolDirectory = System.IO.Path.Combine(
            userHome,
            ".nuget",
            "packages",
            $"{ClangSharpToolId}.{rid}",
            toolVersion,
            "tools",
            "any",
            rid);

        if (!Directory.Exists(toolDirectory))
        {
            context.Warning($"ClangSharp tool runtime not found at '{toolDirectory}'.");
            return;
        }

        processSettings.EnvironmentVariables ??= new Dictionary<string, string>(StringComparer.Ordinal);

        processSettings.EnvironmentVariables["DYLD_LIBRARY_PATH"] =
            PrependPathEnvironmentVariable("DYLD_LIBRARY_PATH", toolDirectory);
        processSettings.EnvironmentVariables["DYLD_FALLBACK_LIBRARY_PATH"] =
            PrependPathEnvironmentVariable("DYLD_FALLBACK_LIBRARY_PATH", toolDirectory);

        context.Information($"Using bundled libclang from: {toolDirectory}");
    }

    private static string PrependPathEnvironmentVariable(string name, string prefix)
    {
        var existing = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(existing) ? prefix : $"{prefix}:{existing}";
    }

    private static string TryReadToolVersion(string rootDirectory)
    {
        var manifestPath = System.IO.Path.Combine(rootDirectory, "dotnet-tools.json");
        if (!File.Exists(manifestPath))
        {
            return null;
        }

        using var stream = File.OpenRead(manifestPath);
        using var document = JsonDocument.Parse(stream);

        if (!document.RootElement.TryGetProperty("tools", out var tools))
        {
            return null;
        }

        if (!tools.TryGetProperty(ClangSharpToolId, out var toolDefinition))
        {
            return null;
        }

        if (!toolDefinition.TryGetProperty("version", out var versionElement))
        {
            return null;
        }

        return versionElement.GetString();
    }

    private static void AddMacOSSdkArguments(BuildContext context, List<string> clangSharpArgs)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var sdkRoot = ResolveMacOSSdkRoot();
        if (string.IsNullOrWhiteSpace(sdkRoot))
        {
            context.Warning("Unable to resolve macOS SDK root; ClangSharp may fail to find standard C headers.");
            return;
        }

        clangSharpArgs.Add("--additional");
        clangSharpArgs.Add("-isysroot");
        clangSharpArgs.Add("--additional");
        clangSharpArgs.Add(sdkRoot);

        context.Information($"Using macOS SDK root: {sdkRoot}");
    }

    private static string ResolveMacOSSdkRoot()
    {
        var sdkRootFromEnvironment = Environment.GetEnvironmentVariable("SDKROOT");
        if (!string.IsNullOrWhiteSpace(sdkRootFromEnvironment) && Directory.Exists(sdkRootFromEnvironment))
        {
            return sdkRootFromEnvironment;
        }

        var knownSdkLocations = new[]
        {
            "/Library/Developer/CommandLineTools/SDKs/MacOSX.sdk",
            "/Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX.sdk"
        };

        foreach (var location in knownSdkLocations)
        {
            if (Directory.Exists(location))
            {
                return location;
            }
        }

        return null;
    }
}
