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
        private static bool _hasServiceCollectionChanged;

        /// <summary>
        /// Sets the internal <see cref="IServiceCollection"/> used to resolve our static instance of
        /// <see cref="IRazorTemplateEngine"/>.
        /// </summary>
        /// <param name="services"></param>
        internal static void UseServiceCollection(IServiceCollection services)
        {
            _services = services;
            _hasServiceCollectionChanged = true;
        }

        /// <summary>
        /// Creates the cache of RazorViewToStringRenderer. If already initialized, re-initializes.
        /// </summary>
        [Obsolete("This method is now marked as obsolete and no longer used. It will be removed in the upcoming versions. You can safely remove it and it doesn't affect any functionality.")]
        public static void Initialize()
        {

        }

        /// <summary>
        /// Creates an instance of <see cref="RazorTemplateEngine"/> using an internal <see cref="ServiceCollection"/>.
        /// </summary>
        /// <returns></returns>
        private static IRazorTemplateEngine CreateInstance()
        {
            var services = _services;

            // was AddRazorTemplating UseServiceCollection called?
            if (services is null)
            {
                // caller may not be using DI directly like in Azure Functions or WPF, 
                // create our own service collection and register everything required.
                services = new ServiceCollection();
                services.AddRazorTemplating();
            }

            var instance = new RazorTemplateEngineRenderer(services.BuildServiceProvider());
            return instance;
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view data</param>
        /// <returns></returns>
        public async static Task<string> RenderAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            if (_hasServiceCollectionChanged)
            {
                _instance = new Lazy<IRazorTemplateEngine>(CreateInstance, true);
                _hasServiceCollectionChanged = false;
            }
            return await _instance.Value.RenderAsync(viewName, viewModel, viewBagOrViewData).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view data</param>
        /// <returns></returns>
        [Obsolete("This method with generic type param is now obsolete and it will be removed in the upcoming versions. Please use the overload method without generic parameter instead.")]
        public async static Task<string> RenderAsync<TModel>(string viewName, object viewModel, Dictionary<string, object> viewBagOrViewData)
        {
            return await _instance.Value.RenderAsync(viewName, viewModel, viewBagOrViewData).ConfigureAwait(false);
        }
    }
}
