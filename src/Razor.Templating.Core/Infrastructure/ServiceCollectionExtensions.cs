using Microsoft.Extensions.DependencyInjection.Extensions;
using Razor.Templating.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRazorTemplating(this IServiceCollection services)
        {
            RazorViewToStringRendererFactory.ServiceCollection = services;
            RazorTemplateEngine.Initialize();

            services.TryAddTransient<IRazorTemplateEngine, RazorTemplateEngineInstance>();
        }
    }
}
