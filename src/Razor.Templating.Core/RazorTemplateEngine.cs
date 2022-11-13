using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public static class RazorTemplateEngine
    {
        private static Lazy<IRazorTemplateEngine> _instance = new(CreateInstance, true);
        private static IServiceCollection? _services;

        /// <summary>
        /// Sets the internal <see cref="IServiceCollection"/> used to resolve our static instance of
        /// <see cref="IRazorTemplateEngine"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="InvalidOperationException">The service has already been initiaized.</exception>
        internal static void UseServiceCollection(IServiceCollection services)
        {
            _services = services;

            // Whenever a new service collection is set, rebuild the IRazorTemplateEngine instance
            // This is expected to be called only once during the application startup
            _instance = new(CreateInstance, true);
        }

        /// <summary>
        /// Creates the cache of RazorViewToStringRenderer. If already initialized, re-initializes.
        /// </summary>
        [Obsolete("This method is now marked as obsolete and no longer used. It will be removed in the upcoming versions. You can safely remove it and it doesn't affect any functionality.")]
        public static void Initialize()
        {
            // TODO: Remove this method in v2.0.0
        }

        /// <summary>
        /// Creates an instance of <see cref="RazorTemplateEngine"/> using an internal <see cref="ServiceCollection"/>.
        /// </summary>
        /// <returns></returns>
        private static IRazorTemplateEngine CreateInstance()
        {
            // was AddRazorTemplating UseServiceCollection called?
            if (_services is null)
            {
                // caller may not be using DI directly like in Azure Functions or WPF, 
                // create our own service collection and register everything required.
                _services = new ServiceCollection();
                _services.AddRazorTemplating();
            }

            return _services.BuildServiceProvider().GetRequiredService<IRazorTemplateEngine>();
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view data</param>
        /// <returns>Rendered HTML string of the view</returns>
        public async static Task<string> RenderAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            return await _instance.Value.RenderAsync(viewName, viewModel, viewBagOrViewData).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders the Razor View(.cshtml) Without Layout to String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view bag or view data</param>
        /// <returns>Rendered HTML string of the view</returns>
        public async static Task<string> RenderPartialAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            return await _instance.Value.RenderPartialAsync(viewName, viewModel, viewBagOrViewData).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders the Razor View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view data</param>
        /// <returns>Rendered HTML string of the view</returns>
        [Obsolete("This method with generic type param is now obsolete and it will be removed in the upcoming versions. Please use the overload method without generic parameter instead.")]
        public async static Task<string> RenderAsync<TModel>(string viewName, object viewModel, Dictionary<string, object> viewBagOrViewData)
        {
            // TODO: Remove this method in v2.0.0
            return await _instance.Value.RenderAsync(viewName, viewModel, viewBagOrViewData).ConfigureAwait(false);
        }
    }
}
