using System;
using System.Collections.Generic;
using System.IO;

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
    public DirectoryPath BindingsDirectory => SourceDirectory.Combine("ThorVGSharp").Combine("Interop");

}

[TaskName("bindings")]
[TaskDescription("Generates P/Invoke bindings from ThorVG C API using ClangSharp")]
public sealed class GenerateBindingsTask : FrostingTask<BuildContext>
{
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

        context.EnsureDirectoryExists(context.BindingsDirectory);
        context.CleanDirectory(context.BindingsDirectory);

        // Build the ClangSharp command line arguments
        List<string> clangSharpArgs =
        [
            "--file", headerPath.FullPath,
            "--output", outputPath,
            "--namespace", "ThorVGSharp.Interop",
            "--libraryPath", "thorvg",
            "--methodClassName", "NativeMethods",
            "--with-access-specifier", "*=Internal",
            "--config", "latest-codegen",
            "--config", "multi-file",
            "--config", "generate-native-bitfield-attribute",
            "--config", "generate-macro-bindings",
            "--config", "generate-aggressive-inlining",
            "--config", "generate-file-scoped-namespaces",
            "--config", "generate-helper-types",
            "--config", "generate-unmanaged-constants",
            "--exclude", "TVG_API",
            "--exclude", "TVG_DEPRECATED",
        ];

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

        var exitCode = context.StartProcess("dotnet", new ProcessSettings
        {
            Arguments = arguments,
            WorkingDirectory = context.RootDirectory
        });

        if (exitCode != 0)
        {
            throw new Exception($"ClangSharpPInvokeGenerator failed with exit code {exitCode}");
        }

        context.Information($"Bindings generated successfully in: {outputPath}");
    }
}
