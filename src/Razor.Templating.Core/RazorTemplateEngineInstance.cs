using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public class RazorTemplateEngineInstance : IRazorTemplateEngine
    {
        private IServiceScopeFactory? _rendererServiceScopeFactory;

        private RazorViewToStringRenderer? _renderViewToStringRenderer;
        public RazorTemplateEngineInstance(IServiceScopeFactory sf)
        {
            _renderViewToStringRenderer = sf.CreateScope().ServiceProvider.GetService<RazorViewToStringRenderer>();
        }

        /// <summary>
        /// Creates the cache of RazorViewToStringRenderer. If already initialized, re-initializes.
        /// </summary>
        public void Initialize()
        {
            _rendererServiceScopeFactory = null;
            GetRendererServiceScopeFactory();
        }

        /// <summary>
        /// Get the ServiceScopeFactory object from static property cache if already exists else creates a new object.
        /// </summary>
        /// <returns></returns>
        private IServiceScopeFactory GetRendererServiceScopeFactory()
        {
            return _rendererServiceScopeFactory ??= new RazorViewToStringRendererFactory().CreateRendererServiceScopeFactory();
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <returns>Rendered string from the view</returns>
        public async Task<string> RenderAsync([DisallowNull] string viewName)
        {
            if (_renderViewToStringRenderer is null)
            {
                using var serviceScope = GetRendererServiceScopeFactory().CreateScope();
                _renderViewToStringRenderer = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            }
            return await _renderViewToStringRenderer.RenderViewToStringAsync<object>(viewName, default!).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Strongly typed object </param>
        /// <returns></returns>
        public async Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model)
        {
            if (_renderViewToStringRenderer is null)
            {
                using var serviceScope = GetRendererServiceScopeFactory().CreateScope();
                _renderViewToStringRenderer = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            }

            return await _renderViewToStringRenderer.RenderViewToStringAsync(viewName, model).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Strongly typed object</param>
        /// <param name="viewData">ViewData</param>
        /// <returns></returns>
        public async Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model, [DisallowNull] Dictionary<string, object> viewData)
        {
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            foreach (var keyValuePair in viewData.ToList())
            {
                viewDataDictionary.Add(keyValuePair!);
            }

            if (_renderViewToStringRenderer is null)
            {
                using var serviceScope = GetRendererServiceScopeFactory().CreateScope();
                _renderViewToStringRenderer = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            }
            return await _renderViewToStringRenderer.RenderViewToStringAsync(viewName, model, viewDataDictionary).ConfigureAwait(false);
        }
    }
}
