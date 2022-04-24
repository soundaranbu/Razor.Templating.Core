using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace Razor.Templating.Core
{
    public class RazorTemplatingOptions
    {
        public MvcRazorRuntimeCompilationOptions MvcRazorRuntimeCompilationOptions { get; } = new MvcRazorRuntimeCompilationOptions();
    }
}
