using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public static class RazorTemplateEngine
    {
        private static readonly Lazy<IRazorTemplateEngine> _instance = new(CreateInstance, true);

        /// <summary>
        /// Creates an instance of <see cref="RazorTemplateEngine"/> using an internal <see cref="ServiceCollection"/>.
        /// </summary>
        /// <returns></returns>
        private static IRazorTemplateEngine CreateInstance()
        {
            ServiceCollection services = new();
            services.AddRazorTemplating();

            RazorTemplateEngineImpl instance = new (services.BuildServiceProvider());
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
            return await _instance.Value.RenderAsync(viewName, model, viewData).ConfigureAwait(false);
        }
    }
}
