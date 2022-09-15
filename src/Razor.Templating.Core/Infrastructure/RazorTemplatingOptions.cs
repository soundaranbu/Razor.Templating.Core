using Razor.Templating.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public class RazorTemplatingOptions
    {
        /// <summary>
        /// If set to false, the static <see cref="RazorTemplateEngine"/> will not be registered with the service collection.
        /// Generally would set this in unit tests where AddRazorTemplating may be called multiple times.
        /// Defaults to true.
        /// </summary>
        public bool UseStaticRazorTemplateEngine { get; set; } = true;
    }
}
