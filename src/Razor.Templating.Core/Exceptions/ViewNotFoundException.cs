using System;

namespace Razor.Templating.Core.Exceptions;

public class ViewNotFoundException : InvalidOperationException
{
    public ViewNotFoundException(string message) : base(message){
        
    }

}