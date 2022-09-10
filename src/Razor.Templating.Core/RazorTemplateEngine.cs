using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public static class RazorTemplateEngine
    {
        private static readonly Lazy<IRazorTemplateEngine> Instance = new(CreateInstance, true);
        private static IServiceCollection? _services;

        /// <summary>
        /// Sets the internal <see cref="IServiceCollection"/> used to resolve our static instance of
        /// <see cref="IRazorTemplateEngine"/>.
        /// </summary>
        /// <param name="services"></param>
        internal static void UseServiceCollection(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Creates an instance of <see cref="RazorTemplateEngine"/> using an internal <see cref="ServiceCollection"/>.
        /// </summary>
        /// <returns></returns>
        private static IRazorTemplateEngine CreateInstance()
        {
            IServiceCollection? services = _services;

            // was AddRazorTemplating UseServiceCollection called?
            if (services is null)
            {
                // caller may not be using DI directly like in Azure Functions or WPF, 
                // create our own service collection and register everything required.
                services = new ServiceCollection();
                services.AddRazorTemplating();
            }

            var instance = new RazorTemplateEngineImpl(services.BuildServiceProvider());
            return instance;
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Optional model data</param>
        /// <param name="viewData">Optional view data</param>
        /// <returns></returns>
        public async static Task<string> RenderAsync([DisallowNull] string viewName, object? model = null, Dictionary<string, object>? viewData = null)
        {
            return await Instance.Value.RenderAsync(viewName, model, viewData).ConfigureAwait(false);
        }
    }
}
