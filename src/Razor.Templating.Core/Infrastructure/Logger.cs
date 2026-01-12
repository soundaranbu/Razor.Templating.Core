using System;
using System.Diagnostics;

namespace Razor.Templating.Core.Infrastructure;

internal static class Logger
{
    [Conditional("DEBUG")]
    public static void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{Constants.LibraryIdentifier}: {message}");
        Console.ResetColor();
    }
}
