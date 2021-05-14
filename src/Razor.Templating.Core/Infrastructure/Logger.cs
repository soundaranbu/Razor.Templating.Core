using System;
using System.Collections.Generic;
using System.Text;

namespace Razor.Templating.Core.Infrastructure
{
    internal class Logger
    {
        public static void Log(string message)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Constants.LibraryIdentifier}: {message}");
            Console.ResetColor();
#endif
        }
    }
}
