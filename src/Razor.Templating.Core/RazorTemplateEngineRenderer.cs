using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    internal sealed class RazorTemplateEngineRenderer : IRazorTemplateEngine
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateEngine"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is null.</exception>
        public RazorTemplateEngineRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Renders the Razor View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view bag or view data</param>
        /// <returns>Rendered HTML string of the view</returns>
        public async Task<string> RenderAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            var viewDataDictionary = GetViewDataDictionaryFromViewBagOrViewData(viewBagOrViewData);

            using var serviceScope = _serviceProvider.CreateScope();
            var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            return await renderer.RenderViewToStringAsync(viewName, viewModel, viewDataDictionary, isMainPage: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders the Razor View(.cshtml) Without Layout to String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view bag or view data</param>
        /// <returns>Rendered HTML string of the view</returns>
        public async Task<string> RenderPartialAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            var viewDataDictionary = GetViewDataDictionaryFromViewBagOrViewData(viewBagOrViewData);

            using var serviceScope = _serviceProvider.CreateScope();
            var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            return await renderer.RenderViewToStringAsync(viewName, viewModel, viewDataDictionary, isMainPage: false).ConfigureAwait(false);
        }

        private static ViewDataDictionary GetViewDataDictionaryFromViewBagOrViewData(Dictionary<string, object>? viewBagOrViewData)
        {
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            foreach (var keyValuePair in viewBagOrViewData ?? new())
            {
                viewDataDictionary.Add(keyValuePair!);
            }
            return viewDataDictionary;
        }

        public async Task<(bool ViewExists, string? RenderedView)> TryRenderAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            try
            {
                var renderedView = await RenderAsync(viewName, viewModel, viewBagOrViewData);
                return (true, renderedView);
            }
            catch (ViewNotFoundException)
            {
            }

            return (false, null);
        }

        public async Task<(bool ViewExists, string? RenderedView)> TryRenderPartialAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            try
            {
                var renderedView = await RenderPartialAsync(viewName, viewModel, viewBagOrViewData);
                return (true, renderedView);
            }
            catch (ViewNotFoundException)
            {
            }

            return (false, null);
        }
    }
}