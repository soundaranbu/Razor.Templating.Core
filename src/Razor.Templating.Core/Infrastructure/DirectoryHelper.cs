using System.Diagnostics;
using System.IO;

namespace Razor.Templating.Core.Infrastructure;

internal static class DirectoryHelper
{
    /// <summary>
    /// Returns the path of the main executable file using which the application is started
    /// </summary>
    /// <returns></returns>
    internal static string? GetMainExecutableDirectory()
    {
        using var processModule = Process.GetCurrentProcess().MainModule;
        return Path.GetDirectoryName(processModule?.FileName);
    }

    /// <summary>
    /// Get the web root directory where the static content resides. This is to add support for MVC applications
    /// If the webroot directory doesn't exist, set the path to assembly base directory.
    /// </summary>
    /// <param name="assembliesBaseDirectory"></param>
    /// <returns></returns>
    internal static string GetWebRootDirectory(string assembliesBaseDirectory)
    {
        var webRootDirectory = Path.Combine(assembliesBaseDirectory, "wwwroot");
        if (!Directory.Exists(webRootDirectory))
        {
            webRootDirectory = assembliesBaseDirectory;
        }

        return webRootDirectory;
    }
}