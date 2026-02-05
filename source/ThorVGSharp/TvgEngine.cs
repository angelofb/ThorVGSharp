using System.Runtime.InteropServices;

using ThorVGSharp.Interop;

namespace ThorVGSharp;

/// <summary>
/// Initializes and terminates the ThorVG engine.
/// </summary>
public static class TvgEngine
{
    /// <summary>
    /// Initializes the ThorVG engine with the specified number of threads.
    /// </summary>
    /// <param name="threads">Number of threads to use. Pass 0 for automatic detection.</param>
    /// <exception cref="TvgException">Thrown when initialization fails.</exception>
    public static void Initialize(uint threads = 0)
    {
        if (IsInitialized)
            return;

        var result = NativeMethods.tvg_engine_init(threads);
        TvgResultHelper.CheckResult(result, "engine initialization");
        IsInitialized = true;
        ThreadCount = threads;
    }

    /// <summary>
    /// Terminates the ThorVG engine and releases all resources.
    /// </summary>
    /// <exception cref="TvgException">Thrown when termination fails.</exception>
    public static void Terminate()
    {
        if (!IsInitialized)
            return;

        var result = NativeMethods.tvg_engine_term();
        TvgResultHelper.CheckResult(result, "engine termination");
        IsInitialized = false;
        ThreadCount = 0;
    }

    /// <summary>
    /// Gets a value indicating whether the engine is initialized.
    /// </summary>
    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the number of threads the engine was initialized with.
    /// Returns 0 if not initialized or if automatic detection was used.
    /// </summary>
    public static uint ThreadCount { get; private set; }

    /// <summary>
    /// Gets the ThorVG version information.
    /// </summary>
    public static unsafe string Version
    {
        get
        {
            uint major, minor, micro;
            sbyte* versionPtr;
            NativeMethods.tvg_engine_version(&major, &minor, &micro, &versionPtr);
            return Marshal.PtrToStringUTF8((IntPtr)versionPtr) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the ThorVG version as major, minor, and micro numbers.
    /// </summary>
    public static unsafe (uint major, uint minor, uint micro) GetVersion()
    {
        uint major, minor, micro;
        sbyte* version;
        NativeMethods.tvg_engine_version(&major, &minor, &micro, &version);
        return (major, minor, micro);
    }
}
