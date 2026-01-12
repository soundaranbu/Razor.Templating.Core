using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Razor.Templating.Core;

public static class RazorTemplateEngine
{
    private static Lazy<IRazorTemplateEngine> _instance = new(CreateInstance, true);
    private static IServiceCollection? _services;

    /// <summary>
    /// Sets the internal <see cref="IServiceCollection"/> used to resolve our static instance of
    /// <see cref="IRazorTemplateEngine"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="InvalidOperationException">The service has already been initialized.</exception>
    internal static void UseServiceCollection(IServiceCollection services)
    {
        _services = services;

        // Whenever a new service collection is set, rebuild the IRazorTemplateEngine instance
        // This is expected to be called only once during the application startup
        _instance = new(CreateInstance, true);
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
    /// Renders the Razor View(.cshtml) Without Layout to String. This method does not throw exception when View is not found.
    /// </summary>
    /// <param name="viewName"></param>
    /// <param name="viewModel"></param>
    /// <param name="viewBagOrViewData"></param>
    /// <returns></returns>
    public async static Task<(bool ViewExists, string? RenderedView)> TryRenderPartialAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
    {
        return await _instance.Value.TryRenderPartialAsync(viewName, viewModel, viewBagOrViewData).ConfigureAwait(false);
    }
}
