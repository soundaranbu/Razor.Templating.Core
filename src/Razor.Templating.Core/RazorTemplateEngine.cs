using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public static class RazorTemplateEngine
    {
        private static readonly Lazy<IRazorTemplateEngine> Instance = new(CreateInstance, true);

        /// <summary>
        /// Creates an instance of <see cref="RazorTemplateEngine"/> using an internal <see cref="ServiceCollection"/>.
        /// </summary>
        /// <returns></returns>
        private static IRazorTemplateEngine CreateInstance()
        {
            ServiceCollection services = new();
            services.AddRazorTemplating();

            RazorTemplateEngineImpl instance = new(services.BuildServiceProvider());
            return instance;
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view bag or view data</param>
        /// <returns></returns>
        public async static Task<string> RenderAsync(string viewName, object? model = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            return await Instance.Value.RenderAsync(viewName, model, viewBagOrViewData).ConfigureAwait(false);
        }
    }
}
