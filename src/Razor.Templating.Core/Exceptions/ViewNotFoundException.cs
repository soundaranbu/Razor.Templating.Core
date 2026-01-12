using System;

namespace Razor.Templating.Core.Exceptions;

public class ViewNotFoundException(string message) : InvalidOperationException(message)
{
}