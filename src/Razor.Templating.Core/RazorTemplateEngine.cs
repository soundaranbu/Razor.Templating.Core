using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public class RazorTemplateEngine
    {
        private static IServiceScopeFactory? _rendererServiceScopeFactory;

        /// <summary>
        /// Creates the cache of RazorViewToStringRenderer. If already initialized, re-initializes.
        /// </summary>
        public static void Initialize()
        {
            _rendererServiceScopeFactory = null;
            GetRendererServiceScopeFactory();
        }

        /// <summary>
        /// Get the ServiceScopeFactory object from static property cache if already exists else creates a new object.
        /// </summary>
        /// <returns></returns>
        private static IServiceScopeFactory GetRendererServiceScopeFactory()
        {
            return _rendererServiceScopeFactory ??= new RazorViewToStringRendererFactory().CreateRendererServiceScopeFactory();
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Strongly typed object</param>
        /// <param name="viewData">ViewData</param>
        /// <returns></returns>
        public async static Task<string> RenderAsync([DisallowNull] string viewName, object? model = null, Dictionary<string, object>? viewData = null)
        { 
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            foreach (var keyValuePair in viewData ?? new())
            {
                viewDataDictionary.Add(keyValuePair!);
            }

            using var serviceScope = GetRendererServiceScopeFactory().CreateScope();
            var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            return await renderer.RenderViewToStringAsync(viewName, model, viewDataDictionary).ConfigureAwait(false);
        }
    }
}
